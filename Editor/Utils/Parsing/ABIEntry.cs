using Newtonsoft.Json;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Utils
    {
        namespace Parsing
        {
            /// <summary>
            ///   Contains the data of an ABI entry.
            /// </summary>
            public class ABIEntry
            {
                /// <summary>
                ///   The type of the entry.
                /// </summary>
                [JsonProperty("type")]
                public string Type { get; set; }
                
                /// <summary>
                ///   The name of the entry (it only makes sense
                ///   for the events and functions).
                /// </summary>
                [JsonProperty("name")]
                public string Name { get; set; }
                
                /// <summary>
                ///   The state mutability: pure and view (they're
                ///   'call' invocations), nonpayable (called with
                ///   'send' invocation) and payable (same but also
                ///   sending the 'amount' to pay with).
                /// </summary>
                [JsonProperty("stateMutability")]
                public string StateMutability { get; set; }
                
                /// <summary>
                ///   The arguments (for events and functions).
                /// </summary>
                [JsonProperty("inputs")]
                public ABIParameter[] Inputs { get; set; }
                
                /// <summary>
                ///   The return values (for functions).
                /// </summary>
                [JsonProperty("outputs")]
                public ABIParameter[] Outputs { get; set; }
                
                /// <summary>
                ///   For events, whether it is anonymous or not.
                /// </summary>
                [JsonProperty("anonymous")]
                public bool Anonymous { get; set; }
            }
        }
    }
}