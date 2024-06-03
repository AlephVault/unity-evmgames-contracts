using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Types.Events
    {
        /// <summary>
        ///   Processes the base ERC-20 events.
        /// </summary>
        public class ERC20EventsProcessorsSet
        {
            /// <summary>
            ///   The account to track funds for.
            /// </summary>
            public readonly string Account;

            /// <summary>
            ///   The current amount in the token.
            /// </summary>
            public BigInteger Amount { get; private set; }

            /// <summary>
            ///   An event for when the amount changes.
            /// </summary>
            public event Action<BigInteger, BigInteger> AmountChanged;

            // The underlying events processors.
            private EventsProcessors processors;
            
            private Task ProcessIncomingTransfer(EventLog<TransferEventDTO> log)
            {
                Amount += log.Event.Value;
                try
                {
                    AmountChanged?.Invoke(Amount, log.Event.Value);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return Task.CompletedTask;
            }

            private Task ProcessOutgoingTransfer(EventLog<TransferEventDTO> log)
            {
                Amount -= log.Event.Value;
                try
                {
                    AmountChanged?.Invoke(Amount, -log.Event.Value);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return Task.CompletedTask;
            }
            
            private EventLogsProcessor MakeTransferFromEventsProcessor(
                ERC20Contract contract, string account, BlockParameter fromBlock
            ) {
                return new EventLogsProcessor<TransferEventDTO>(
                    contract.MakeTransferEventsWorker(
                        @event => @event.CreateFilterInput(account), fromBlock
                    ),
                    ProcessOutgoingTransfer
                );
            }
            
            private EventLogsProcessor MakeTransferToEventsProcessor(
                ERC20Contract contract, string account, BlockParameter fromBlock
            ) {
                return new EventLogsProcessor<TransferEventDTO>(
                    contract.MakeTransferEventsWorker(@event => @event.CreateFilterInput(
                        null, new []{ account }
                    ), fromBlock), ProcessIncomingTransfer
                );
            }
            
            private EventLogsProcessor[] MakeProcessorsFor(
                ERC20EventsProcessorsSet obj, ERC20Contract contract, string account,
                BlockParameter fromBlock
            ) {
                contract = contract ?? throw new ArgumentNullException(nameof(contract));
                return new EventLogsProcessor[] {
                    MakeTransferFromEventsProcessor(contract, account, fromBlock),
                    MakeTransferToEventsProcessor(contract, account, fromBlock),
                };
            }
            
            public ERC20EventsProcessorsSet(
                ERC20Contract contract, string account,
                BigInteger initialAmount, BlockParameter fromBlock = null
            ) {
                Account = account;
                Amount = initialAmount;
                processors = new EventsProcessors(MakeProcessorsFor(
                    this, contract, account, fromBlock
                ));
            }

            /// <summary>
            ///   Runs the 
            /// </summary>
            /// <param name="toBlock"></param>
            /// <returns></returns>
            public Task Run(BlockParameter toBlock = null)
            {
                return processors.Process(toBlock);
            }
        }
    }
}