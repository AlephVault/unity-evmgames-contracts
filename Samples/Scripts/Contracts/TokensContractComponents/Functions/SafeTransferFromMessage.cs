using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Functions
        {
            [Function("safeTransferFrom")]
            public class SafeTransferFromMessage : FunctionMessage
            {
                [Parameter("address", "from", 1)]
                public string From { get; set; }
                
                [Parameter("address", "to", 2)]
                public string To { get; set; }
                
                [Parameter("uint256", "id", 3)]
                public BigInteger Id { get; set; }
                
                [Parameter("uint256", "value", 4)]
                public BigInteger Value { get; set; }
                
                [Parameter("bytes", "data", 5)]
                public byte Data { get; set; }
            }
        }
    }
}
