using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using UnityEngine;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Types.Events
    {
        /// <summary>
        ///   Processes the incoming events.
        /// </summary>
        public abstract class EventLogsProcessor
        {
            /// <summary>
            ///   Gets handlers related to events until some related block.
            /// </summary>
            /// <param name="toBlock">The target block</param>
            /// <returns>The list of handlers</returns>
            public abstract Task<List<EventLogHandler>> GetHandlers(BlockParameter toBlock = null);
        }

        /// <summary>
        ///   Processes the incoming events of certain type.
        /// </summary>
        /// <typeparam name="EventLogType">The event type</typeparam>
        public abstract class EventLogsProcessor<EventLogType> : EventLogsProcessor
            where EventLogType : IEventDTO, new()
        {
            private EventsWorker<EventLogType> _worker;
            private Func<EventLog<EventLogType>, Task> _handler;

            /// <summary>
            ///   The event handler class for this processor.
            ///   It executes a callback.
            /// </summary>
            private class LocalEventLogHandler : EventLogHandler<EventLogType>
            {
                private Func<EventLog<EventLogType>, Task> _handler;

                public LocalEventLogHandler(
                    Func<EventLog<EventLogType>, Task> handler,
                    EventLog<EventLogType> log
                ) : base(log) {
                    _handler = handler;
                }

                public override Task Handle()
                {
                    return _handler(EventLog);
                }
            }
            
            public EventLogsProcessor(
                EventsWorker<EventLogType> worker, Func<EventLog<EventLogType>, Task> handler
            )
            {
                _handler = handler ?? throw new ArgumentNullException(nameof(handler));
                _worker = worker ?? throw new ArgumentNullException(nameof(worker));
            }
            
            private List<EventLogHandler> GetHandlers(List<EventLog<EventLogType>> eventLogs)
            {
                return (
                    from log in eventLogs
                    select new LocalEventLogHandler(_handler, log) as EventLogHandler
                ).ToList();
            }

            /// <summary>
            ///   Gets handlers related to events until some related block.
            /// </summary>
            /// <param name="toBlock">The target block</param>
            /// <returns>The list of handlers</returns>
            public override async Task<List<EventLogHandler>> GetHandlers(BlockParameter toBlock = null)
            {
                return GetHandlers(await _worker.GetEvents(toBlock));
            }
        }

        /// <summary>
        ///   Coordinates many processors to go together.
        /// </summary>
        public class EventsProcessors
        {
            // The instantiated processors.
            private EventLogsProcessor[] _processors;
            
            /// <summary>
            ///   Takes a list of processors and processes it properly.
            ///   Ensure all the processor instances are different and
            ///   refer different event workers (however, the underlying
            ///   events can safely be the same, perhaps with different
            ///   arguments).
            /// </summary>
            /// <param name="processors">The processors</param>
            /// <exception cref="ArgumentNullException">One or more of the processors were null</exception>
            public EventsProcessors(params EventLogsProcessor[] processors)
            {
                for (int index = 0; index < processors.Length; index++)
                {
                    if (processors[index] == null) throw new ArgumentNullException($"processor #${index}");
                }
                _processors = processors;
            }

            private async Task<List<EventLogHandler>> GetMergedHandlers(BlockParameter toBlock = null)
            {
                List<EventLogHandler> handlers = new List<EventLogHandler>();
                foreach (var processor in _processors)
                {
                    handlers.AddRange(await processor.GetHandlers(toBlock));
                }
                
                handlers.Sort((a, b) =>
                {
                    if (a.Log.BlockNumber.Value > b.Log.BlockNumber.Value)
                    {
                        return 1;
                    }
                    if (a.Log.BlockNumber.Value < b.Log.BlockNumber.Value)
                    {
                        return -1;
                    }
                    if (a.Log.TransactionIndex.Value > b.Log.TransactionIndex.Value)
                    {
                        return 1;
                    }
                    if (a.Log.TransactionIndex.Value < b.Log.TransactionIndex.Value)
                    {
                        return -1;
                    }
                    if (a.Log.LogIndex.Value > b.Log.LogIndex.Value)
                    {
                        return 1;
                    }
                    if (a.Log.LogIndex.Value < b.Log.LogIndex.Value)
                    {
                        return -1;
                    }

                    return 0;
                });

                return handlers;
            }

            /// <summary>
            ///   Gets a sorted bulk of events and processes it according to its handler.
            /// </summary>
            /// <param name="toBlock">The final block to get events until</param>
            public async Task Process(BlockParameter toBlock = null)
            {
                if ((toBlock?.ParameterType ?? BlockParameter.BlockParameterType.latest) !=
                    BlockParameter.BlockParameterType.blockNumber)
                {
                    Debug.LogWarning(
                        "The current toBlock specified value does not have a specific " +
                        "number. This can lead to eventual race conditions when retrieving the " +
                        "events as they might differ. While the difference might be not that " +
                        "much, it can lead to inconsistent processed states. To get the latest " +
                        "block number, use a web3 client: var latestBlockNumber = " +
                        "await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();"
                    );

                    foreach (var handler in await GetMergedHandlers(toBlock))
                    {
                        await handler.Handle();
                    }
                }
            }
        }
    }
}