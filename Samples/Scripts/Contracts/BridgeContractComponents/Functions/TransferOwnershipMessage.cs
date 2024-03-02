using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("transferOwnership")]
            public class TransferOwnershipMessage : FunctionMessage
            {
                [Parameter("address", "newOwner", 1)]
                public string NewOwner { get; set; }
            }
        }
    }
}
