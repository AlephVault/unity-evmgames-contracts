using System;
using System.Linq;
using Newtonsoft.Json;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Utils
    {
        namespace Parsing
        {
            /// <summary>
            ///   Contains the data of an ABI parameter.
            /// </summary>
            public class ABIParameter
            {
                /// <summary>
                ///   The type of the parameter.
                /// </summary>
                [JsonProperty("type")]
                public string Type { get; set; }
                
                /// <summary>
                ///   The name of the parameter.
                /// </summary>
                [JsonProperty("name")]
                public string Name { get; set; }
                
                /// <summary>
                ///   The components of this parameter. This only
                ///   makes sense when the type is tuple, tuple[]
                ///   or tuple[N] with a positive N.
                /// </summary>
                [JsonProperty("components")]
                public ABIParameter[] Components { get; private set; }

                /// <summary>
                ///   For events, whether it is indexed (i.e. one
                ///   of the 3 extra topics) or not.
                /// </summary>
                [JsonProperty("indexed")]
                public bool Indexed;
            }
        }
    }
}