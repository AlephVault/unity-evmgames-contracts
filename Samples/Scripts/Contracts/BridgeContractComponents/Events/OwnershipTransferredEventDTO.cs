using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Events
        {
            [Event("OwnershipTransferred")]
            public class OwnershipTransferredEventDTO : IEventDTO
            {
                [Parameter("address", "previousOwner", 1, true)]
                public string PreviousOwner { get; set; }
                
                [Parameter("address", "newOwner", 2, true)]
                public string NewOwner { get; set; }
            }
        }
    }
}
