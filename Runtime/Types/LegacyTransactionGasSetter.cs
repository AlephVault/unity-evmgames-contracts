using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.TransactionTypes;
using Nethereum.Web3;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Types
    {
        /// <summary>
        ///   This legacy gas setter uses just Gas and Gas Price,
        ///   without knowledge of EIP-1559.
        /// </summary>
        public abstract class LegacyTransactionGasSetter : ITransactionGasSetter
        {
            // The current gas estimator function.
            private Func<Task<HexBigInteger>> gasEstimator;

            // The current gas price estimator.
            private Func<Task<HexBigInteger>> gasPriceEstimator;
            
            /// <summary>
            ///   The amount of gas to set.
            /// </summary>
            public BigInteger Gas { get; protected set; }
            
            /// <summary>
            ///   The price of gas to set.
            /// </summary>
            public BigInteger GasPrice { get; protected set; }

            /// <summary>
            ///   Launches the user interface. This must start
            ///   the UI, and perhaps few other things.
            /// </summary>
            protected abstract Task<bool> Launch();

            /// <summary>
            ///   Updates the gas from a new estimation.
            /// </summary>
            protected async Task EstimateGas()
            {
                Gas = await gasEstimator();
            }

            /// <summary>
            ///   Updates the gas price from a new estimation.
            /// </summary>
            protected async Task EstimateGasPrice()
            {
                GasPrice = await gasPriceEstimator();
            }
            
            /// <inheritdoc cref="ITransactionGasSetter.SetGasSettings"/>
            public async Task SetGasSettings(Web3 web3, Func<Task<HexBigInteger>> estimatedGas, FunctionMessage message)
            {
                // Sets the gas estimator and refreshes the estimation
                // for the very first time. Further estimations can be
                // invoked again later.
                gasEstimator = estimatedGas;
                await EstimateGas();

                // Sets the gas price estimator and refreshes the estimation
                // for the very first time. Further estimations can be invoked
                // again later.
                gasPriceEstimator = () => web3.Eth.GasPrice.SendRequestAsync();
                await EstimateGasPrice();

                // It opens the dialog. In the meantime, it can call
                // EstimateGas or EstimateGasPrice again as needed.
                bool ok = await Launch();
                if (!ok) throw new OperationCanceledException();
                
                // In the end, the parameters are updated in the transaction.
                message.Gas = Gas;
                message.GasPrice = GasPrice;
                message.TransactionType = TransactionType.Legacy.AsByte();
            }
        }
    }
}