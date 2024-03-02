using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephhVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Functions
        {
            [Function("mint")]
            public class MintMessage : FunctionMessage
            {
                [Parameter("address", "to", 1)]
                public string To { get; set; }
                
                [Parameter("uint256", "id", 2)]
                public BigInteger Id { get; set; }
            }
        }
    }
}
