using System;
using System.Collections.Generic;
using AlephVault.Unity.EVMGames.Contracts.Utils.Parsing;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Utils
    {
        namespace Models
        {
            /// <summary>
            ///   Describes a parsed ABI event and whatever
            ///   is needed to properly match it to generated
            ///   NEthereum code.
            /// </summary>
            public class ABIEventModel
            {
                // EVM only supports 3 indexed arguments.
                private const int MaxIndexedArgs = 3;

                /// <summary>
                ///   An argument to the event.
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
                    ///   The ABI argument type.
                    /// </summary>
                    public string Type { get; }
                    
                    /// <summary>
                    ///   The C# field type.
                    /// </summary>
                    public string FieldType { get; }
                    
                    /// <summary>
                    ///   Whether it is indexed.
                    /// </summary>
                    public bool Indexed { get; }
                    
                    /// <summary>
                    ///   The order of the argument.
                    /// </summary>
                    public int Order { get; }
                    
                    public Argument(
                        string name, string fieldName, string type, string fieldType,
                        bool indexed, int order
                    )
                    {
                        Name = name;
                        FieldName = fieldName;
                        Type = type;
                        FieldType = fieldType;
                        Indexed = indexed;
                        Order = order;
                    }
                }

                // The arguments for the event.
                private Dictionary<string, Argument> arguments = new();

                /// <summary>
                ///   The arguments.
                /// </summary>
                public IReadOnlyDictionary<string, Argument> Arguments => arguments;

                // The C# types of the indexed arguments (topics).
                public readonly List<(string, string)> IndexedArguments = new();
                
                /// <summary>
                ///   The ABI event name.
                /// </summary>
                public string Name { get; }
                
                /// <summary>
                ///   The C# class name.
                /// </summary>
                public string ClassName { get; }

                /// <summary>
                ///   Makes the model from the ABI entry.
                /// </summary>
                /// <param name="abiEntry">The entry</param>
                public ABIEventModel(ABIEntry abiEntry)
                {
                    Name = abiEntry.Name;
                    ClassName = Sanitizer.SanitizeContractEventName(Name);
                    foreach (ABIParameter parameter in abiEntry.Inputs)
                    {
                        string abiType = parameter.Type;
                        (string baseABIType, int[] dims) = ABITypes.ParseABIType(abiType);
                        string cSharpType = ABITypes.GetCorrespondingCSharpBaseType(baseABIType);
                        foreach (int _ in dims) cSharpType += "[]";
                        string abiName = parameter.Name;
                        bool indexed = parameter.Indexed;
                        AddArgument(abiName, abiType, cSharpType, indexed);
                    }
                }

                // Adds an argument. Tuples are actually ignored
                // and converted to byte[] instead.
                private void AddArgument(
                    string name, string type, string fieldType, bool indexed
                ) {
                    string fieldName = Sanitizer.SanitizeContractEventOrFunctionArgName(name);
                    if (arguments.ContainsKey(name))
                    {
                        // The argument is already declared. This
                        // is an error and we cannot continue.
                        throw new ArgumentException(
                            $"The argument '{name}' is already defined for the " +
                            $"event '{Name}'", nameof(name)
                        );
                    }
                    
                    if (type.StartsWith("tuple"))
                    {
                        // We fix it to bytes here in both types.
                        // This will force getting all the bytes
                        // and then the user has to make a manual
                        // decode of the contents.
                        type = "bytes";
                        fieldType = "byte[]";
                    }

                    int order = 1 + arguments.Count;
                    if (indexed)
                    {
                        if (IndexedArguments.Count == MaxIndexedArgs)
                        {
                            indexed = false;
                            Debug.LogWarning(
                                $"The event '{Name}' already has {MaxIndexedArgs}. " +
                                $"Argument '{name}' will not be parsed as indexed"
                            );
                        }
                        else
                        {
                            IndexedArguments.Add((name, fieldType));
                        }
                    }

                    arguments.Add(name, new Argument(
                        name, fieldName, type, fieldType, indexed, order
                    ));
                }
            }
        }
    }
}
