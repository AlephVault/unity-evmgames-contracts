using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("onERC1155BatchReceived")]
            public class OnERC1155BatchReceivedMessage : FunctionMessage
            {
                [Parameter("address", "arg0", 1)]
                public string Arg0 { get; set; }
                
                [Parameter("address", "arg1", 2)]
                public string Arg1 { get; set; }
                
                [Parameter("uint256[]", "arg2", 3)]
                public BigInteger[] Arg2 { get; set; }
                
                [Parameter("uint256[]", "arg3", 4)]
                public BigInteger[] Arg3 { get; set; }
                
                [Parameter("bytes", "arg4", 5)]
                public byte Arg4 { get; set; }
            }
        }
    }
}
