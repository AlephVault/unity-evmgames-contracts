using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Functions
        {
            [Function("balanceOf", "uint256")]
            public class BalanceOfMessage : FunctionMessage
            {
                [Parameter("address", "account", 1)]
                public string Account { get; set; }
                
                [Parameter("uint256", "id", 2)]
                public BigInteger Id { get; set; }
            }
        }
    }
}
