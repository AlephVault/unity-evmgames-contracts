using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Events
        {
            [Event("BridgedResourceTypeDefined")]
            public class BridgedResourceTypeDefinedEventDTO : IEventDTO
            {
                [Parameter("uint256", "id", 1, true)]
                public BigInteger Id { get; set; }
                
                [Parameter("uint256", "amountPerUnit", 2, false)]
                public BigInteger AmountPerUnit { get; set; }
            }
        }
    }
}
