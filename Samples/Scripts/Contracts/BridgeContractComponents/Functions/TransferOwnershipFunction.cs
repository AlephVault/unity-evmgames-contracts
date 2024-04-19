using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("transferOwnership")]
            public class TransferOwnershipFunction : FunctionMessage
            {
                [Parameter("address", "newOwner", 1)]
                public string NewOwner { get; set; }
            }
        }
    }
}
