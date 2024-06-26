using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Events
        {
            [Event("TransferBatch")]
            public class TransferBatchEvent : IEventDTO
            {
                [Parameter("address", "operator", 1, true)]
                public string Operator { get; set; }
                
                [Parameter("address", "from", 2, true)]
                public string From { get; set; }
                
                [Parameter("address", "to", 3, true)]
                public string To { get; set; }
                
                [Parameter("uint256[]", "ids", 4, false)]
                public BigInteger[] Ids { get; set; }
                
                [Parameter("uint256[]", "values", 5, false)]
                public BigInteger[] Values { get; set; }
            }
        }
    }
}
