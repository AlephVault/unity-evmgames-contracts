using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;
using AlephVault.Unity.EVMGames.Contracts.Utils;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Types
    {
        /// <summary>
        ///   This base contract can set parameters to make a call.
        /// </summary>
        public abstract class BaseContract
        {
            /// <summary>
            ///   The contract.
            /// </summary>
            public ContractHandler Contract { get; }
            
            /// <summary>
            ///   The Web3 client.
            /// </summary>
            public Web3 Web3 { get; }

            /// <summary>
            ///   The transaction gas setter.
            /// </summary>
            public ITransactionGasSetter TransactionGasSetter { get; }

            protected BaseContract(
                Web3 web3, string address, ITransactionGasSetter transactionGasSetter = null
            )
            {
                web3 = web3 ?? throw new ArgumentNullException(nameof(web3));
                if (!address.IsValidEthereumAddressHexFormat())
                {
                    throw new ArgumentException("Invalid address");
                }

                TransactionGasSetter = transactionGasSetter;
                Contract = web3.Eth.GetContractHandler(address);
                Web3 = web3;
            }

            /// <summary>
            ///   Invokes a method using Call. This works when the method
            ///   returns a single value, instead of a tuple.
            /// </summary>
            /// <param name="args">The arguments</param>
            /// <typeparam name="FuncType">The method's args type</typeparam>
            /// <typeparam name="ResultType">The result type</typeparam>
            /// <returns>The call result</returns>
            public async Task<ResultType> Call<FuncType, ResultType>(FuncType args = null, BlockParameter blockParameter = null)
                where FuncType : FunctionMessage, new()
            {
                return await Contract.QueryAsync<FuncType, ResultType>(args, blockParameter);
            }

            /// <summary>
            ///   Invokes a method using Call. This allows calling methods
            ///   returning tuples.
            /// </summary>
            /// <param name="args">The arguments</param>
            /// <typeparam name="FuncType">The method's args type</typeparam>
            /// <typeparam name="ResultType">The result type</typeparam>
            /// <returns>The call result</returns>
            public async Task<ResultType> CallMulti<FuncType, ResultType>(FuncType args = null, BlockParameter blockParameter = null)
                where FuncType : FunctionMessage, new()
                where ResultType : IFunctionOutputDTO, new()
            {
                return await Contract.QueryDeserializingToObjectAsync<FuncType, ResultType>(args, blockParameter);
            }

            /// <summary>
            ///   Sets the gas settings into a transaction about to being sent.
            /// </summary>
            private Task SetGasSettings(FunctionMessage message, Func<Task<HexBigInteger>> estimateGas)
            {
                if (TransactionGasSetter == null)
                {
                    throw new InvalidOperationException(
                        "No gasSetter was specified on construction. Transactions cannot be sent"
                    );
                }
                return TransactionGasSetter.SetGasSettings(Web3, estimateGas, message);
            }

            /// <summary>
            ///   Invokes a method using Send.
            /// </summary>
            /// <param name="args">The arguments</param>
            /// <param name="value">The amount to send</param>
            /// <returns>The transaction</returns>
            public async Task<TransactionReceipt> Send<FuncType>(FuncType args, BigInteger value = default)
                where FuncType : FunctionMessage, new()
            {
                // Setting the value to pay.
                if (value == default) value = new BigInteger(0);
                
                // Setting the from address.
                string from = Shortcuts.GetAccount(Web3).Address;

                // Retrieving the transaction handler.
                IContractTransactionHandler<FuncType> handler = Web3.Eth.GetContractTransactionHandler<FuncType>();
                
                // Configuring the transaction settings. Gas is
                // set from the callback, while the from address
                // and the amount to send are set from current
                // parameters and Web3 client.
                Task<HexBigInteger> EstimateGas() => handler.EstimateGasAsync(Contract.ContractAddress, args);

                await SetGasSettings(args, EstimateGas);
                args.FromAddress = from;
                args.AmountToSend = value;

                // Send the request.
                return await handler.SendRequestAndWaitForReceiptAsync(Contract.ContractAddress, args);
            }

            /// <summary>
            ///   Makes an event worker for a given event type.
            ///   This event has no extra topics.
            /// </summary>
            /// <typeparam name="EventType">The event type</typeparam>
            /// <param name="fromBlock">The starting block</param>
            /// <returns>The event worker</returns>
            public EventsWorker<EventType> MakeEventsWorker<EventType>(
                Func<Event<EventType>, BlockParameter, NewFilterInput> filterMaker,
                BlockParameter fromBlock = null
            ) where EventType : IEventDTO, new()
            {
                return new EventsWorker<EventType>(
                    Web3.Eth.GetEvent<EventType>(),
                    filterMaker, fromBlock
                );
            }
        }
    }
}