using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("onERC1155Received")]
            public class OnERC1155ReceivedFunction : FunctionMessage
            {
                [Parameter("address", "arg0", 1)]
                public string Arg0 { get; set; }
                
                [Parameter("address", "from", 2)]
                public string From { get; set; }
                
                [Parameter("uint256", "id", 3)]
                public BigInteger Id { get; set; }
                
                [Parameter("uint256", "value", 4)]
                public BigInteger Value { get; set; }
                
                [Parameter("bytes", "data", 5)]
                public byte[] Data { get; set; }
            }
        }
    }
}
