using System;
using System.Collections.Generic;
using System.Linq;
using AlephVault.Unity.EVMGames.Contracts.Utils.Parsing;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Utils
    {
        namespace Models
        {
            /// <summary>
            ///   Describes a parsed ABI function and whatever
            ///   is needed to properly match it to generated
            ///   NEthereum code (i.e. includes both input and
            ///   output arguments).
            /// </summary>
            public class ABIFunctionModel
            {
                /// <summary>
                ///   Tells that the function is a call with a
                ///   void return type and will not be generated.
                /// </summary>
                public class VoidResultUnsupportedOnCall : Exception
                {
                    public VoidResultUnsupportedOnCall() : base() {}
                    public VoidResultUnsupportedOnCall(string message) : base(message) {}
                }

                // These names are reserved.
                private HashSet<string> reservedNames = new HashSet<string>(
                new []{
                    "AmountToSend", "Gas", "GasPrice", "FromAddress", "Nonce",
                    "MaxFeePerGas", "MaxPriorityFeePerGas", "TransactionType",
                    "AccessList"
                });
                
                /// <summary>
                ///   The mutability type.
                /// </summary>
                public enum MutabilityType { Send, PaidSend, Call }
                
                /// <summary>
                ///   The mutability.
                /// </summary>
                public MutabilityType Mutability { get; }
                
                /// <summary>
                ///   The ABI function name.
                /// </summary>
                public string Name { get; }
                
                /// <summary>
                ///   The C# method name.
                /// </summary>
                public string MethodName { get; }
                
                /// <summary>
                ///   The C# class name. Used to distinguish overloads,
                ///   since the method name per se may collide.
                /// </summary>
                public string ClassName { get; }

                /// <summary>
                ///   An argument to the function.
                /// </summary>
                public class Argument
                {
                    /// <summary>
                    ///   The ABI argument name.
                    /// </summary>
                    public string Name { get; }
                    
                    /// <summary>
                    ///   The C# field name.
                    /// </summary>
                    public string FieldName { get; }
                    
                    /// <summary>
                    ///   The C# method arg name.
                    /// </summary>
                    public string MethodArgName { get; }
                    
                    /// <summary>
                    ///   The ABI argument type.
                    /// </summary>
                    public string Type { get; }
                    
                    /// <summary>
                    ///   The C# field type. Might be an array.
                    /// </summary>
                    public string FieldType { get; }
                    
                    /// <summary>
                    ///   The order of the argument.
                    /// </summary>
                    public int Order { get; }
                    
                    /// <summary>
                    ///   Whether this argument is complex or primitive.
                    /// </summary>
                    public bool IsComplex { get; }
                    
                    public Argument(
                        string name, string fieldName, string methodArgName,
                        string type, string fieldType, int order, bool isComplex
                    )
                    {
                        Name = name;
                        FieldName = fieldName;
                        MethodArgName = methodArgName;
                        Type = type;
                        FieldType = fieldType;
                        Order = order;
                        IsComplex = isComplex;
                    }
                }

                /// <summary>
                ///   An extra argument type, defined as a new class.
                ///   This stands both for input and output arguments.
                /// </summary>
                public class ArgumentType
                {
                    /// <summary>
                    ///   The class name.
                    /// </summary>
                    public string ClassName { get; }

                    /// <summary>
                    ///   The arguments.
                    /// </summary>
                    public Dictionary<string, Argument> Fields { get; } = new();

                    public ArgumentType(string className)
                    {
                        ClassName = className;
                    }
                }

                // The input argument types for the function.
                private List<ArgumentType> inputArgumentTypes = new();

                /// <summary>
                ///   The input argument types.
                /// </summary>
                public IReadOnlyCollection<ArgumentType> InputArgumentTypes => inputArgumentTypes;

                // The input arguments for the function.
                private Dictionary<string, Argument> inputArguments = new();

                /// <summary>
                ///   The input arguments.
                /// </summary>
                public IReadOnlyDictionary<string, Argument> InputArguments => inputArguments;

                // The output argument types for the function.
                private List<ArgumentType> outputArgumentTypes = new();

                /// <summary>
                ///   The output argument types.
                /// </summary>
                public IReadOnlyCollection<ArgumentType> OutputArgumentTypes => outputArgumentTypes;

                // The output arguments from the  function.
                private Dictionary<string, Argument> outputArguments = new();

                /// <summary>
                ///   The output arguments.
                /// </summary>
                public IReadOnlyDictionary<string, Argument> OutputArguments => outputArguments;

                /// <summary>
                ///   Makes the model from the ABI entry.Collection
                /// </summary>
                /// <param name="abiEntry">The entry</param>
                /// <param name="overloadCount">The previous overloads</param>
                public ABIFunctionModel(ABIEntry abiEntry, int overloadCount)
                {
                    string name = abiEntry.Name;
                    string mutability = abiEntry.StateMutability;
                    
                    switch (mutability)
                    {
                        case "pure":
                        case "view":
                            Mutability = MutabilityType.Call;
                            break;
                        case "nonpayable":
                            Mutability = MutabilityType.Send;
                            break;
                        case "payable":
                            Mutability = MutabilityType.PaidSend;
                            break;
                        default:
                            throw new ArgumentException(
                                $"Invalid mutability value: {mutability}. Expected: " +
                                $"pure, view, payable or nonpayable", nameof(mutability)
                            );
                    }
                    Name = name;
                    MethodName = Sanitizer.SanitizeContractFunctionName(name);
                    ClassName = overloadCount == 1 ? MethodName : $"{MethodName}V{overloadCount}";
                    
                    if (abiEntry.Outputs.Length != 0)
                    {
                        if (Mutability != MutabilityType.Call)
                        {
                            Debug.LogWarning(
                                $"An ABI entry for function {abiEntry.Name} has a mutability " +
                                "other than pure/view and has return values. They will be ignored, " +
                                "since they cannot be retrieved from front-ends. No type classes " +
                                "will be generated for them"
                            );
                        }
                        else
                        {
                            // We don't know beforehand whether we'll have a class or not,
                            // since perhaps there's only one field and it is not a tuple.
                            //
                            // Nevertheless, since we might build the type itself
                            PopulateArguments(
                                abiEntry, abiEntry.Outputs, outputArguments, outputArgumentTypes,
                                ClassName + "Output", false
                            );
                        }
                    }
                    else if (Mutability == MutabilityType.Call)
                    {
                        throw new VoidResultUnsupportedOnCall(
                            $"The function {abiEntry.Name} has a '{abiEntry.StateMutability}' " +
                            $"mutability but no result type. It will be ignored"
                        );
                    }

                    if (abiEntry.Inputs.Length != 0)
                    {
                        PopulateArguments(
                            abiEntry, abiEntry.Inputs, inputArguments, inputArgumentTypes,
                            GetInputType(), true
                        );
                    }
                }

                // Makes an argument.
                private Argument MakeArgument(
                    string abiName, string abiType, int index, string baseName,
                    bool avoidCollision = false
                )
                {
                    string name = string.IsNullOrWhiteSpace(abiName) ? $"arg{index}" : abiName;
                    string fieldName = Sanitizer.SanitizeContractEventOrFunctionArgName(name);
                    if (avoidCollision && reservedNames.Contains(fieldName))
                    {
                        fieldName += "_";
                    }
                    string methodArgName = fieldName.Substring(0, 1).ToLower() + fieldName.Substring(1);
                    (string baseABIType, int[] dims) = ABITypes.ParseABIType(abiType);
                    string fieldType = ABITypes.GetCorrespondingCSharpBaseType(baseABIType);
                    bool isComplex = fieldType == "tuple";
                    if (isComplex)
                    {
                        fieldType = baseName + fieldName;
                    }
                    int length = dims.Length;
                    for (int idx = 0; idx < length; idx++) fieldType += "[]"; 
                    
                    return new Argument(
                        name, fieldName, methodArgName,
                        abiType, fieldType, index + 1,
                        isComplex
                    );
                }

                // Registers a custom type given a name and its parameters.
                private void RegisterCustomType(
                    string functionName, List<ArgumentType> argumentTypes,
                    string baseName, ABIParameter[] components
                )
                {
                    ArgumentType argumentType = new ArgumentType(baseName);
                    int length = components.Length;
                    for (int index = 0; index < length; index++)
                    {
                        ABIParameter parameter = components[index];
                        Argument argument = MakeArgument(
                            parameter.Name, parameter.Type, index, baseName
                        );
                        if (argumentType.Fields.ContainsKey(argument.Name))
                        {
                            throw new ArgumentException(
                                $"While defining a function {functionName}, " +
                                $"this parameter name is already used: {argument.Name}"
                            );
                        }
                        argumentType.Fields.Add(argument.Name, argument);
                        if (argument.IsComplex)
                        {
                            RegisterCustomType(
                                functionName, argumentTypes, baseName + argument.FieldName,
                                parameter.Components
                            );
                        }
                    }
                    argumentTypes.Add(argumentType);
                }

                // This one is used to generate the input/output arguments
                // for the function. It involves also the type definition
                // and registration.
                private void PopulateArguments(
                    ABIEntry abiEntry, ABIParameter[] abiArgs,
                    Dictionary<string, Argument> arguments,
                    List<ArgumentType> argumentTypes,
                    string baseName, bool avoidCollision
                )
                {
                    int length = abiArgs.Length;
                    for (int index = 0; index < length; index++)
                    {
                        ABIParameter parameter = abiArgs[index];
                        Argument argument = MakeArgument(
                            parameter.Name, parameter.Type, index, baseName, avoidCollision
                        );
                        if (arguments.ContainsKey(argument.Name))
                        {
                            throw new ArgumentException(
                                $"While defining a function {abiEntry.Name}, " +
                                $"this parameter name is already used: {argument.Name}"
                            );
                        }
                        arguments.Add(argument.Name, argument);
                        if (argument.IsComplex)
                        {
                            RegisterCustomType(
                                abiEntry.Name, argumentTypes,
                                baseName + argument.FieldName, parameter.Components
                            );
                        }
                    }
                }
                
                /// <summary>
                ///   The result type. It might be "void" for [Paid]Send
                ///   calls only, a primitive type (if only one return
                ///   value is specified), or a compound output DTO type.
                /// </summary>
                /// <returns>The name of the output type</returns>
                public string GetReturnType()
                {
                    switch (outputArguments.Count)
                    {
                        case 0:
                            return "void";
                        case 1:
                            string type = outputArguments.Values.First().Type;
                            return type.StartsWith("tuple")
                                ? ClassName + "Output"
                                : outputArguments.Values.First().FieldType;
                        default:
                            return ClassName + "Output";
                    }
                }

                /// <summary>
                ///   The result type to use in the Function() tag.
                /// </summary>
                /// <returns>The type</returns>
                public string GetFunctionMessageAttributeReturnType()
                {
                    switch (outputArguments.Count)
                    {
                        case 0:
                            return "";
                        case 1:
                            string type = outputArguments.Values.First().Type;
                            return type.StartsWith("tuple")
                                ? $"typeof({ClassName}Output)"
                                : $"\"{type}\"";
                        default:
                            return $"typeof({ClassName}Output)";
                    }
                }

                /// <summary>
                ///   Tells whether this function model needs
                ///   to define a custom return type.
                /// </summary>
                public bool HasCustomReturnType()
                {
                    switch (outputArguments.Count)
                    {
                        case 0:
                            return false;
                        case 1:
                            return outputArguments.Values.First().Type.StartsWith("tuple");
                        default:
                            return true;
                    }
                }

                /// <summary>
                ///   There will always be a defined input type.
                /// </summary>
                /// <returns>The name of the input type</returns>
                public string GetInputType()
                {
                    return ClassName + "Function";
                }
            }
        }
    }
}
