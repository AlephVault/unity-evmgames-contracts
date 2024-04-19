using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [FunctionOutput]
            public class BridgedResourceTypesOutput : IFunctionOutputDTO
            {
                [Parameter("uint256", "status", 1)]
                public BigInteger Status { get; set; }
                
                [Parameter("uint256", "amountPerUnit", 2)]
                public BigInteger AmountPerUnit { get; set; }
            }
        }
    }
}
