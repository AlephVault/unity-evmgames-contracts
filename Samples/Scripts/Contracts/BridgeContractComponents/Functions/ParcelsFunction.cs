using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("parcels", typeof(ParcelsOutput))]
            public class ParcelsFunction : FunctionMessage
            {
                [Parameter("bytes32", "arg0", 1)]
                public byte[] Arg0 { get; set; }
            }
        }
    }
}
