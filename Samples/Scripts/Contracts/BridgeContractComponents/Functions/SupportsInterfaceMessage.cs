using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("supportsInterface", "bool")]
            public class SupportsInterfaceMessage : FunctionMessage
            {
                [Parameter("bytes4", "interfaceId", 1)]
                public byte[] InterfaceId { get; set; }
            }
        }
    }
}
