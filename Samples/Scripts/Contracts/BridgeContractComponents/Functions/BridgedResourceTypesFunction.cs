using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace BridgeContractComponents
    {
        namespace Functions
        {
            [Function("bridgedResourceTypes", typeof(BridgedResourceTypesOutput))]
            public class BridgedResourceTypesFunction : FunctionMessage
            {
                [Parameter("uint256", "arg0", 1)]
                public BigInteger Arg0 { get; set; }
            }
        }
    }
}
