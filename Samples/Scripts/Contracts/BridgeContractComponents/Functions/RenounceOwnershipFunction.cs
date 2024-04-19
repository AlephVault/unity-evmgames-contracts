using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("renounceOwnership")]
            public class RenounceOwnershipFunction : FunctionMessage
            {

            }
        }
    }
}
