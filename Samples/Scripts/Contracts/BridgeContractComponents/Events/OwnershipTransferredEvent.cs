using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Events
        {
            [Event("OwnershipTransferred")]
            public class OwnershipTransferredEvent : IEventDTO
            {
                [Parameter("address", "previousOwner", 1, true)]
                public string PreviousOwner { get; set; }
                
                [Parameter("address", "newOwner", 2, true)]
                public string NewOwner { get; set; }
            }
        }
    }
}
