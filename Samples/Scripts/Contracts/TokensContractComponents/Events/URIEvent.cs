using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace AlephVault.Unity.EVMGames.Contracts.Samples.Contracts
{
    namespace TokensContractComponents
    {
        namespace Events
        {
            [Event("URI")]
            public class URIEvent : IEventDTO
            {
                [Parameter("string", "value", 1, false)]
                public string Value { get; set; }
                
                [Parameter("uint256", "id", 2, true)]
                public BigInteger Id { get; set; }
            }
        }
    }
}
