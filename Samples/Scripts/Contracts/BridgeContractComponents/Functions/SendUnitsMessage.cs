using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("sendUnits")]
            public class SendUnitsMessage : FunctionMessage
            {
                [Parameter("address", "_to", 1)]
                public string To { get; set; }
                
                [Parameter("uint256", "_id", 2)]
                public BigInteger Id { get; set; }
                
                [Parameter("uint256", "_units", 3)]
                public BigInteger Units { get; set; }
            }
        }
    }
}
