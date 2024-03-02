using System.Collections.Generic;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Utils
    {
        namespace Models
        {
            /// <summary>
            ///   Describes a parsed ABI contract and whatever
            ///   is needed to properly match it to generated
            ///   NEthereum code (i.e. includes all the items
            ///   that were parsed from the ABI).
            /// </summary>
            public class ABIContractModel
            {
                /// <summary>
                ///   The list of registered events.
                /// </summary>
                public readonly List<ABIEventModel> Events = new();

                /// <summary>
                ///   The list of registered functions.
                /// </summary>
                public readonly List<ABIFunctionModel> Functions = new();
            }
        }
    }
}
