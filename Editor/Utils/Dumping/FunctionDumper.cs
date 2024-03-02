using System;
using System.Collections.Generic;
using AlephVault.Unity.Boilerplates.Utils;
using AlephVault.Unity.EVMGames.Contracts.Utils.Models;
using UnityEditor;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Utils
    {
        namespace Dumping
        {
            /// <summary>
            ///   Dumps appropriate parameter files and also dumps the
            ///   proper function interfaces using them.
            /// </summary>
            public static class FunctionDumper
            {
                private const string indentation2 = "        ";
                private const string indentation3 = "            ";
                private const string indentation4 = "                ";

                /// <summary>
                ///   Dumps a function model.
                /// </summary>
                /// <param name="contractName">The contract name (without -Contract suffix)</param>
                /// <param name="functionModel">The function model</param>
                public static void DumpFunction(
                    string contractName, ABIFunctionModel functionModel
                )
                {
                    // "{FunctionName}Message"
                    string inputType = functionModel.GetInputType();
                    // Something like "long", "BigInteger", "string", "byte[]",
                    // or "{FunctionMessage}Output". It may be "void", but that
                    // means that no output type should be accounted for. It
                    // will be used as typeof(xxx) when generating the input
                    // type class.
                    string outputType = functionModel.GetReturnType();
                    // Whether an output type should be accounted for.
                    bool hasOutputType = functionModel.HasCustomReturnType();

                    // Dump everything.
                    if (hasOutputType) DumpFunctionOutputTypeFile(contractName, functionModel, outputType);
                    DumpFunctionInputTypeFile(contractName, functionModel, inputType, hasOutputType, outputType);
                }

                private static string MakeParameters(IReadOnlyDictionary<string, ABIFunctionModel.Argument> arguments)
                {
                    List<string> lines = new();
                    foreach (var pair in arguments)
                    {
                        lines.Add($"{indentation4}[Parameter(\"{pair.Value.Type}\", \"{pair.Value.Name}\", {pair.Value.Order})]");
                        lines.Add($"{indentation4}public {pair.Value.FieldType} {pair.Value.FieldName} {{ get; set; }}");
                        lines.Add(indentation4);
                    }
                    return string.Join("\n", lines).TrimEnd();
                }

                private static string MakeExtraTypes(IReadOnlyCollection<ABIFunctionModel.ArgumentType> types)
                {
                    List<string> lines = new();
                    foreach (ABIFunctionModel.ArgumentType type in types)
                    {
                        lines.Add($"{indentation3}public class {type.ClassName}");
                        lines.Add($"{indentation3}{{");
                        lines.Add(MakeParameters(type.Fields));
                        lines.Add($"{indentation3}}}");
                        lines.Add(indentation3);
                    }

                    return string.Join("\n", lines);
                }
                
                // Dumps the input type file.
                private static void DumpFunctionInputTypeFile(
                    string contractName, ABIFunctionModel functionModel,
                    string inputType, bool hasOutputType, string outputType
                )
                {
                    // The templates directory.
                    string directory = "Packages/com.alephvault.unity.evmgames.contracts/" +
                                       "Editor/Utils/Dumping/Templates";

                    // The Message template.
                    TextAsset abiFunctionMessageTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(
                        directory + "/ABIFunctionMessage.cs.txt"
                    );

                    string functionReturn = functionModel.GetFunctionMessageAttributeReturnType();
                    Dictionary<string, string> replacements = new Dictionary<string, string>
                    {
                        {"CONTRACT_NAME", contractName},
                        {"FUNCTION_ABI_NAME", functionModel.Name},
                        {"EXTRA_TYPES", MakeExtraTypes(functionModel.InputArgumentTypes)},
                        {"FUNCTION_RETURN", functionReturn != "" ? $", {functionReturn}": ""},
                        {"CONTENT", MakeParameters(functionModel.InputArguments)}
                    };

                    new Boilerplate()
                        .IntoDirectory("Scripts")
                            .IntoDirectory("Contracts")
                                .IntoDirectory(contractName + "ContractComponents")
                                    .IntoDirectory("Functions")
                                        .Do(Boilerplate.InstantiateScriptCodeTemplate(
                                            abiFunctionMessageTemplate, inputType, replacements
                                        ))
                                    .End()
                                .End()
                            .End()
                        .End();
                }
                
                // Dumps the output type file.
                private static void DumpFunctionOutputTypeFile(
                    string contractName, ABIFunctionModel functionModel,
                    string outputType
                )
                {
                    // The templates directory.
                    string directory = "Packages/com.alephvault.unity.evmgames.contracts/" +
                                       "Editor/Utils/Dumping/Templates";

                    // The OutputDTO template.
                    TextAsset abiFunctionOutputTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(
                        directory + "/ABIFunctionOutputDTO.cs.txt"
                    );
                    
                    Dictionary<string, string> replacements = new Dictionary<string, string>
                    {
                        {"CONTRACT_NAME", contractName},
                        {"EXTRA_TYPES", MakeExtraTypes(functionModel.OutputArgumentTypes)},
                        {"CONTENT", MakeParameters(functionModel.OutputArguments)}
                    };

                    new Boilerplate()
                        .IntoDirectory("Scripts")
                            .IntoDirectory("Contracts")
                                .IntoDirectory(contractName + "ContractComponents")
                                    .IntoDirectory("Functions")
                                        .Do(Boilerplate.InstantiateScriptCodeTemplate(
                                            abiFunctionOutputTemplate, outputType, replacements
                                        ))
                                    .End()
                                .End()
                            .End()
                        .End();
                }
                
                /// <summary>
                ///   Makes the definition of a standard method
                ///   for a contract class.
                /// </summary>
                /// <param name="functionModel">The function model to make the definition from</param>
                /// <returns>The involved code</returns>
                public static string MakeFunctionMethod(ABIFunctionModel functionModel)
                {
                    switch (functionModel.Mutability)
                    {
                        case ABIFunctionModel.MutabilityType.PaidSend:
                            return MakeSendMethod(functionModel, true);
                        case ABIFunctionModel.MutabilityType.Send:
                            return MakeSendMethod(functionModel);
                        case ABIFunctionModel.MutabilityType.Call:
                            return MakeCallMethod(functionModel);
                        default:
                            throw new ArgumentException(
                                "The given function model has an invalid mutability",
                                nameof(functionModel)
                            );
                    }
                }

                // Builds the parameters for the methods.
                private static string GetParameters(ABIFunctionModel functionModel)
                {
                    List<string> parameters = new List<string>();
                    foreach (var inputArgumentPair in functionModel.InputArguments)
                    {
                        // Please note: no field argument / method argument will match
                        // the name of a reserved field like AmountToSend.
                        parameters.Add($"{inputArgumentPair.Value.FieldType} {inputArgumentPair.Value.MethodArgName}");
                    }

                    if (functionModel.Mutability == ABIFunctionModel.MutabilityType.Call)
                    {
                        parameters.Add("BlockParameter blockParameter = null");
                    }

                    return string.Join(", ", parameters);
                }

                // Makes a Send method, perhaps a paid one.
                private static string MakeSendMethod(ABIFunctionModel functionModel, bool paid = false)
                {
                    string parameters = GetParameters(functionModel);
                    if (paid) parameters = parameters != "" ? $"{parameters}, BigInteger amountToSend" : "BigInteger amountToSend";
                    List<string> lines = new List<string>();
                    lines.Add($"{indentation2}public Task {functionModel.MethodName}({parameters})");
                    lines.Add($"{indentation2}{{");
                    lines.Add($"{indentation2}    return Send(new {functionModel.GetInputType()}");
                    lines.Add($"{indentation2}    {{");
                    foreach (var inputArgumentPair in functionModel.InputArguments)
                    {
                        // Please note: no field argument / method argument will match
                        // the name of a reserved field like AmountToSend.
                        lines.Add($"{indentation2}        {inputArgumentPair.Value.FieldName} = {inputArgumentPair.Value.MethodArgName},");
                    }

                    if (paid)
                    {
                        lines.Add($"{indentation2}        AmountToSend = amountToSend,");
                    }
                    lines.Add($"{indentation2}    }});");
                    lines.Add($"{indentation2}}}");
                    return string.Join("\n", lines);
                }

                // Makes a Call method.
                private static string MakeCallMethod(ABIFunctionModel functionModel)
                {
                    string parameters = GetParameters(functionModel);
                    List<string> lines = new List<string>();
                    string method = functionModel.HasCustomReturnType() ? "CallMulti" : "Call";
                    lines.Add($"{indentation2}public Task<{functionModel.GetReturnType()}> {functionModel.MethodName}({parameters})");
                    lines.Add($"{indentation2}{{");
                    lines.Add($"{indentation2}    return {method}<{functionModel.GetInputType()}, {functionModel.GetReturnType()}>(new {functionModel.GetInputType()}");
                    lines.Add($"{indentation2}    {{");
                    foreach (var inputArgumentPair in functionModel.InputArguments)
                    {
                        // Please note: no field argument / method argument will match
                        // the name of a reserved field like AmountToSend.
                        lines.Add($"{indentation2}        {inputArgumentPair.Value.FieldName} = {inputArgumentPair.Value.MethodArgName},");
                    }
                    lines.Add($"{indentation2}    }}, blockParameter);");
                    lines.Add($"{indentation2}}}");
                    return string.Join("\n", lines);
                }
            }
        }
    }
}