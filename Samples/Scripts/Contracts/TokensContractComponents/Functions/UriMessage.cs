using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Functions
        {
            [Function("uri", "string")]
            public class UriMessage : FunctionMessage
            {
                [Parameter("uint256", "arg0", 1)]
                public BigInteger Arg0 { get; set; }
            }
        }
    }
}
