using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Functions
        {
            [Function("balanceOfBatch", "uint256[]")]
            public class BalanceOfBatchMessage : FunctionMessage
            {
                [Parameter("address[]", "accounts", 1)]
                public string[] Accounts { get; set; }
                
                [Parameter("uint256[]", "ids", 2)]
                public BigInteger[] Ids { get; set; }
            }
        }
    }
}
