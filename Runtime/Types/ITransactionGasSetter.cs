using System;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Types
    {
        /// <summary>
        ///   This interface is used to tell how the gas
        ///   arguments in a transaction will be filled.
        ///   This might even involve user interaction.
        /// </summary>
        public interface ITransactionGasSetter
        {
            /// <summary>
            ///   Configures the gas settings in a given
            ///   function message. This actually starts
            ///   a user interaction most of the times.
            ///   At some point, the message is updated.
            /// </summary>
            /// <param name="web3">The client to use for network estimation</param>
            /// <param name="estimateGas">The estimated gas consumption</param>
            /// <param name="message">The message to configure</param>
            public Task SetGasSettings(Web3 web3, Func<Task<HexBigInteger>> estimateGas, FunctionMessage message);
        }
    }
}