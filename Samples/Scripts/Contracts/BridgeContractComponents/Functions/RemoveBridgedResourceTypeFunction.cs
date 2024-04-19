using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("removeBridgedResourceType")]
            public class RemoveBridgedResourceTypeFunction : FunctionMessage
            {
                [Parameter("uint256", "_id", 1)]
                public BigInteger Id { get; set; }
            }
        }
    }
}
