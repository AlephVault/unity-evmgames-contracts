using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Functions
        {
            [Function("AMOUNT", "uint256")]
            public class AMOUNTMessage : FunctionMessage
            {

            }
        }
    }
}
