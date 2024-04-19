using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("sendTokens")]
            public class SendTokensFunction : FunctionMessage
            {
                [Parameter("address", "_to", 1)]
                public string To { get; set; }
                
                [Parameter("uint256", "_id", 2)]
                public BigInteger Id { get; set; }
                
                [Parameter("uint256", "_value", 3)]
                public BigInteger Value { get; set; }
                
                [Parameter("bytes", "_data", 4)]
                public byte[] Data { get; set; }
            }
        }
    }
}
