using System.Collections.Generic;
using AlephVault.Unity.EVMGames.Contracts.Utils.Models;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Utils
    {
        namespace Parsing
        {
            /// <summary>
            ///   Parses a complete ABI, ignoring constructors, fallbacks,
            ///   errors and receivers.
            /// </summary>
            public static class ABIParser
            {
                /// <summary>
                ///   Parses a list of ABI entries, populating a contract model.
                /// </summary>
                /// <param name="abi">The abi to parse</param>
                /// <param name="contractModel">The model to populate</param>
                public static void Parse(ABIEntry[] abi, ABIContractModel contractModel)
                {
                    Dictionary<string, int> overloadCounts = new();

                    foreach (ABIEntry abiEntry in abi)
                    {
                        if (abiEntry.Type == "function")
                        {
                            if (overloadCounts.TryGetValue(abiEntry.Name, out int count))
                            {
                                overloadCounts[abiEntry.Name] = count + 1;
                            }
                            else
                            {
                                overloadCounts[abiEntry.Name] = 1;
                            }

                            try
                            {
                                contractModel.Functions.Add(new ABIFunctionModel(abiEntry, overloadCounts[abiEntry.Name]));
                            }
                            catch (ABIFunctionModel.VoidResultUnsupportedOnCall e)
                            {
                                Debug.LogWarning(e.Message);
                                overloadCounts[abiEntry.Name] -= 1;
                            }
                        }
                        else if (abiEntry.Type == "event")
                        {
                            contractModel.Events.Add(new ABIEventModel(abiEntry));
                        }
                        else
                        {
                            Debug.LogWarning(
                                $"Ignoring entry of type: {abiEntry.Type} and name: {abiEntry.Name}, " +
                                $"because parsing this type of entry is not supported"
                            );
                        }
                    }
                }
            }
        }
    }
}
