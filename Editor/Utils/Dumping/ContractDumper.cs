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
            ///   Dumps everything related to the contract
            ///   (e.g. events, functions, and the contract
            ///   class itself).
            /// </summary>
            public static class ContractDumper
            {
                /// <summary>
                ///   Dumps an event model.
                /// </summary>
                /// <param name="contractName">The contract name (without -Contract suffix)</param>
                /// <param name="contractModel">The contract model</param>
                public static void DumpContract(
                    string contractName, ABIContractModel contractModel
                )
                {
                    // First, dump all the event classes and function classes.
                    bool eventsDumped = DumpEventTypes(contractName, contractModel);
                    bool functionsDumped = DumpFunctionTypes(contractName, contractModel);

                    // The templates directory.
                    string directory = "Packages/com.alephvault.unity.evmgames.contracts/" +
                                       "Editor/Utils/Dumping/Templates";

                    // The Contract template.
                    TextAsset abiContractTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(
                        directory + "/Contract.cs.txt"
                    );

                    Dictionary<string, string> replacements = new Dictionary<string, string>
                    {
                        {"CONTRACT_NAME", contractName},
                        {"CONTENT", MakeContractContents(contractModel)},
                        {"FUNCTIONS_NAMESPACE", functionsDumped ? $"    using {contractName}ContractComponents.Functions;\n" : ""},
                        {"EVENTS_NAMESPACE", eventsDumped ? $"    using {contractName}ContractComponents.Events;\n" : ""},
                        {"LINE", eventsDumped || functionsDumped ? "    \n" : ""}
                    };

                    new Boilerplate()
                        .IntoDirectory("Scripts", true)
                            .IntoDirectory("Contracts", true)
                                .Do(Boilerplate.InstantiateScriptCodeTemplate(
                                    abiContractTemplate, contractName + "Contract",
                                    replacements
                                ))
                            .End()
                        .End();
                }

                // Makes all the contents for the event classes.
                private static bool DumpEventTypes(string contractName, ABIContractModel contractModel)
                {
                    bool somethingWasDumped = false;
                    foreach (ABIEventModel eventModel in contractModel.Events)
                    {
                        EventDumper.DumpEvent(contractName, eventModel);
                        somethingWasDumped = true;
                    }

                    return somethingWasDumped;
                }
                
                // Makes all the contents for the function classes.
                private static bool DumpFunctionTypes(string contractName, ABIContractModel contractModel)
                {
                    bool somethingWasDumped = false;
                    foreach (ABIFunctionModel functionModel in contractModel.Functions)
                    {
                        FunctionDumper.DumpFunction(contractName, functionModel);
                        somethingWasDumped = true;
                    }

                    return somethingWasDumped;
                }

                // Makes all the contents for the contract class.
                private static string MakeContractContents(ABIContractModel contractModel)
                {
                    List<string> chunks = new List<string>();
                    foreach (ABIFunctionModel functionModel in contractModel.Functions)
                    {
                        chunks.Add(FunctionDumper.MakeFunctionMethod(functionModel));
                    }
                    
                    foreach (ABIEventModel eventModel in contractModel.Events)
                    {
                        chunks.Add(EventDumper.MakeEventWorkerMethod(eventModel));
                    }

                    string result = string.Join("\n\n", chunks);
                    if (result.Trim() != "") result = "\n\n" + result;
                    return result;
                }
            }
        }
    }
}