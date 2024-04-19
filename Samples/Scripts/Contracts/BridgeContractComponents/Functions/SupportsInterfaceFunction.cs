using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("supportsInterface", "bool")]
            public class SupportsInterfaceFunction : FunctionMessage
            {
                [Parameter("bytes4", "interfaceId", 1)]
                public byte[] InterfaceId { get; set; }
            }
        }
    }
}
