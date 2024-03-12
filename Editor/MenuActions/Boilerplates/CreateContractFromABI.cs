using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AlephVault.Unity.EVMGames.Contracts.Utils.Dumping;
using AlephVault.Unity.EVMGames.Contracts.Utils.Models;
using AlephVault.Unity.EVMGames.Contracts.Utils.Parsing;
using AlephVault.Unity.MenuActions.Types;
using AlephVault.Unity.MenuActions.Utils;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace AlephVault.Unity.NetRose
{
    namespace MenuActions
    {
        namespace Boilerplates
        {
            /// <summary>
            ///   This boilerplate function creates a contract out
            ///   of an ABI specification (and a given contract name,
            ///   which is not included in the ABI).
            /// </summary>
            public static class CreateContractFromABI
            {
                /// <summary>
                ///   Utility window used to create the file for a new principal
                ///   objects protocol.
                /// </summary>
                public class CreateContractFromABIWindow : SmartEditorWindow
                {
                    private Regex contractNameCriterion = new Regex("^[A-Z][A-Za-z0-9]*$");
                    
                    // The base name to use.
                    private string contractName = "MyContract";
                    private string abi = "[]";
                    private ABIEntry[] deserializedABI;

                    protected override float GetSmartWidth()
                    {
                        return 700;
                    }

                    protected override void OnAdjustedGUI()
                    {
                        GUIStyle longLabelStyle = MenuActionUtils.GetSingleLabelStyle();

                        EditorGUILayout.LabelField(@"
This utility generates the client side of an EVM contract, with boilerplate code for the methods and events.

For this generation, two things are needed:
1. The name of the contract (starting with upper case A-Z and continuing only with A-Z, a-z or 0-9).
2. The ABI (it must be a JSON string consisting of a valid ABI specification).

You can find the ABI in the directory of your project after compiling (e.g. truffle's build directory).

A lot of files will be directory under the Scripts/Contracts directory, for each generates contract.

WARNING: THIS MIGHT OVERRIDE EXISTING CODE. Always use proper source code management & versioning.
".Trim(), longLabelStyle);

                        // The base name
                        EditorGUILayout.BeginHorizontal();
                        contractName = EditorGUILayout.TextField("Contract name", contractName).Trim();
                        bool validContractName = contractNameCriterion.IsMatch(contractName);
                        if (!validContractName)
                        {
                            EditorGUILayout.LabelField("The contract name is invalid!");
                        }
                        EditorGUILayout.EndHorizontal();

                        // The network object base name
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField("ABI (JSON):");
                        abi = EditorGUILayout.TextArea(abi, GUILayout.Height(140)).Trim();
                        deserializedABI = Deserialize(Encoding.UTF8.GetBytes(abi));
                        if (deserializedABI == null)
                        {
                            EditorGUILayout.LabelField("The ABI is not valid JSON!");
                        }
                        EditorGUILayout.EndVertical();
                        
                        if (validContractName)
                            SmartButton("Generate", () =>
                            {
                                try
                                {
                                    Execute();
                                }
                                catch (Exception e)
                                {
                                    Debug.LogException(e);
                                    EditorGUILayout.LabelField($"Error! See the console for more details");
                                }
                            });
                    }

                    private ABIEntry[] Deserialize(byte[] data)
                    {
                        try
                        {
                            return JsonSerializer.Create().Deserialize<ABIEntry[]>(
                                new JsonTextReader(new StreamReader(new MemoryStream(data)))
                            );
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                            return null;
                        }
                    }

                    private void Execute()
                    {
                        ABIContractModel contractModel = new ABIContractModel();
                        ABIParser.Parse(deserializedABI, contractModel);
                        ContractDumper.DumpContract(contractName, contractModel);
                    }
                }
                
                [MenuItem("Assets/Create/AlephVault/EVM Games/Boilerplates/Contract from ABI", false, 210)]
                public static void ExecuteBoilerplate()
                {
                    CreateContractFromABIWindow window = ScriptableObject.CreateInstance<CreateContractFromABIWindow>();
                    window.titleContent = new GUIContent("EVM Contract generation");
                    window.ShowUtility();
                }
            }
        }
    }
}
