using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Functions
        {
            [Function("setApprovalForAll")]
            public class SetApprovalForAllMessage : FunctionMessage
            {
                [Parameter("address", "operator", 1)]
                public string Operator { get; set; }
                
                [Parameter("bool", "approved", 2)]
                public bool Approved { get; set; }
            }
        }
    }
}
