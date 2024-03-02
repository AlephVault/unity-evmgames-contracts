using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.TransactionTypes;
using Nethereum.Web3;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Types
    {
        /// <summary>
        ///   This legacy gas setter uses Gas, MaxFeePerGas, and
        ///   MaxPriorityFeePerGas.
        /// </summary>
        public abstract class EIP1559TransactionGasSetter : ITransactionGasSetter
        {
            // The current gas estimator function.
            private Func<Task<HexBigInteger>> gasEstimator;

            // The current base gas price estimator.
            private Func<Task<HexBigInteger>> baseGasFeeEstimator;
            
            /// <summary>
            ///   The amount of gas to set.
            /// </summary>
            public BigInteger Gas { get; protected set; }
            
            /// <summary>
            ///   The (global) max fee per gas.
            /// </summary>
            public BigInteger MaxFeePerGas { get; protected set; }
            
            /// <summary>
            ///   The max priority fee per gas.
            /// </summary>
            public BigInteger MaxPriorityFeePerGas { get; protected set; }

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
            ///   Updates the max fee per gas from a new estimation.
            /// </summary>
            protected async Task EstimateMaxFeePerGas()
            {
                MaxFeePerGas = await baseGasFeeEstimator() + MaxPriorityFeePerGas;
            }
            
            public async Task SetGasSettings(Web3 web3, Func<Task<HexBigInteger>> estimatedGas, FunctionMessage message)
            {
                // Sets the gas estimator and refreshes the estimation
                // for the very first time. Further estimations can be
                // invoked again later.
                gasEstimator = estimatedGas;
                await EstimateGas();

                // Sets the gas price estimator and refreshes the estimation
                // for the very first time. Further estimations can be invoked
                // again later. Also, initially no priority will be set.
                MaxPriorityFeePerGas = 0;
                baseGasFeeEstimator = async () =>
                    (await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(
                        BlockParameter.CreateLatest()
                    )).BaseFeePerGas;
                await EstimateMaxFeePerGas();

                // It opens the dialog. In the meantime, it can call
                // EstimateGas or EstimateGasPrice again as needed.
                bool ok = await Launch();
                if (!ok) throw new OperationCanceledException();
                
                // In the end, the parameters are updated in the transaction.
                message.Gas = Gas;
                message.MaxFeePerGas = MaxFeePerGas;
                message.MaxPriorityFeePerGas = MaxPriorityFeePerGas;
                message.TransactionType = TransactionType.EIP1559.AsByte();
            }
        }
    }
}