using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Functions
        {
            [Function("safeBatchTransferFrom")]
            public class SafeBatchTransferFromMessage : FunctionMessage
            {
                [Parameter("address", "from", 1)]
                public string From { get; set; }
                
                [Parameter("address", "to", 2)]
                public string To { get; set; }
                
                [Parameter("uint256[]", "ids", 3)]
                public BigInteger[] Ids { get; set; }
                
                [Parameter("uint256[]", "values", 4)]
                public BigInteger[] Values { get; set; }
                
                [Parameter("bytes", "data", 5)]
                public byte[] Data { get; set; }
            }
        }
    }
}
