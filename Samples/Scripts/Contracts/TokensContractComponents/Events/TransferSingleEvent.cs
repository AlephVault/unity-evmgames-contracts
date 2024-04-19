using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Events
        {
            [Event("TransferSingle")]
            public class TransferSingleEvent : IEventDTO
            {
                [Parameter("address", "operator", 1, true)]
                public string Operator { get; set; }
                
                [Parameter("address", "from", 2, true)]
                public string From { get; set; }
                
                [Parameter("address", "to", 3, true)]
                public string To { get; set; }
                
                [Parameter("uint256", "id", 4, false)]
                public BigInteger Id { get; set; }
                
                [Parameter("uint256", "value", 5, false)]
                public BigInteger Value { get; set; }
            }
        }
    }
}
