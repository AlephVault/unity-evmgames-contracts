using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("defineBridgedResourceType")]
            public class DefineBridgedResourceTypeFunction : FunctionMessage
            {
                [Parameter("uint256", "_id", 1)]
                public BigInteger Id { get; set; }
                
                [Parameter("uint256", "_amountPerUnit", 2)]
                public BigInteger AmountPerUnit { get; set; }
            }
        }
    }
}
