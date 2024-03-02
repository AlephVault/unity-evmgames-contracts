using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Functions
        {
            [Function("isApprovedForAll", "bool")]
            public class IsApprovedForAllMessage : FunctionMessage
            {
                [Parameter("address", "account", 1)]
                public string Account { get; set; }
                
                [Parameter("address", "operator", 2)]
                public string Operator { get; set; }
            }
        }
    }
}
