using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Events
        {
            [Event("ApprovalForAll")]
            public class ApprovalForAllEvent : IEventDTO
            {
                [Parameter("address", "account", 1, true)]
                public string Account { get; set; }
                
                [Parameter("address", "operator", 2, true)]
                public string Operator { get; set; }
                
                [Parameter("bool", "approved", 3, false)]
                public bool Approved { get; set; }
            }
        }
    }
}
