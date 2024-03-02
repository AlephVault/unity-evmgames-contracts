using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [FunctionOutput]
            public class ParcelsOutput : IFunctionOutputDTO
            {
                [Parameter("bool", "created", 1)]
                public bool Created { get; set; }
                
                [Parameter("uint256", "id", 2)]
                public BigInteger Id { get; set; }
                
                [Parameter("uint256", "units", 3)]
                public BigInteger Units { get; set; }
            }
        }
    }
}
