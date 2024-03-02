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
            ///   Dumps appropriate event files and also dumps the
            ///   proper event retrieval methods for the contracts.
            /// </summary>
            public static class EventDumper
            {
                private const string indentation4 = "                "; 
                private const string indentation2 = "        "; 

                // Makes the contents for the EventDTO class.
                private static string MakeEventDTOContents(ABIEventModel eventModel)
                {
                    List<string> lines = new List<string>();
                    
                    foreach (KeyValuePair<string, ABIEventModel.Argument> argument in eventModel.Arguments)
                    {
                        lines.Add($"{indentation4}[Parameter(\"{argument.Value.Type}\", \"{argument.Value.Name}\", {argument.Value.Order}, {argument.Value.Indexed.ToString().ToLower()})]");
                        lines.Add($"{indentation4}public {argument.Value.FieldType} {argument.Value.FieldName} {{ get; set; }}");
                        lines.Add(indentation4);
                    }

                    return string.Join("\n", lines).TrimEnd();
                }

                /// <summary>
                ///   Dumps an event model.
                /// </summary>
                /// <param name="contractName">The contract name (without -Contract suffix)</param>
                /// <param name="eventModel">The event model</param>
                public static void DumpEvent(
                    string contractName, ABIEventModel eventModel
                )
                {
                    // The templates directory.
                    string directory = "Packages/com.alephvault.unity.evmgames.contracts/" +
                                       "Editor/Utils/Dumping/Templates";

                    // The EventDTO template.
                    TextAsset abiEventTemplate = AssetDatabase.LoadAssetAtPath<TextAsset>(
                        directory + "/ABIEventDTO.cs.txt"
                    );
                    
                    Dictionary<string, string> replacements = new Dictionary<string, string>
                    {
                        {"CONTRACT_NAME", contractName},
                        {"EVENT_ABI_NAME", eventModel.Name},
                        {"CONTENT", MakeEventDTOContents(eventModel)}
                    };

                    new Boilerplate()
                        .IntoDirectory("Scripts")
                            .IntoDirectory("Contracts")
                                .IntoDirectory(contractName + "ContractComponents")
                                    .IntoDirectory("Events")
                                        .Do(Boilerplate.InstantiateScriptCodeTemplate(
                                            abiEventTemplate, eventModel.ClassName + "Event",
                                            replacements
                                        ))
                                    .End()
                                .End()
                            .End()
                        .End();
                }
                
                /// <summary>
                ///   Makes the definition of an EventWorker method
                ///   for a contract class.
                /// </summary>
                /// <param name="eventModel">The event model to make the definition from</param>
                /// <returns>The involved code</returns>
                public static string MakeEventWorkerMethod(ABIEventModel eventModel)
                {
                    List<string> lines = new List<string>();
                    lines.Add($"{indentation2}public EventsWorker<{eventModel.ClassName}Event> Make{eventModel.Name}EventsWorker(Func<Event<{eventModel.ClassName}Event>, BlockParameter, NewFilterInput> filterMaker, BlockParameter fromBlock = null)");
                    lines.Add($"{indentation2}{{");
                    lines.Add($"{indentation2}    return MakeEventsWorker(filterMaker, fromBlock);");
                    lines.Add($"{indentation2}}}");
                    return string.Join("\n", lines);
                }
            }
        }
    }
}