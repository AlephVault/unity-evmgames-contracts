using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("removeBridgedResourceType")]
            public class RemoveBridgedResourceTypeMessage : FunctionMessage
            {
                [Parameter("uint256", "_id", 1)]
                public BigInteger Id { get; set; }
            }
        }
    }
}
