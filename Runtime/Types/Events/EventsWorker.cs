using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Types.Events
    {
        /// <summary>
        ///   This iterator lists all the events on certain
        ///   optional indexing criterion.
        /// </summary>
        /// <typeparam name="EventType">The event type</typeparam>
        public class EventsWorker<EventType>
            where EventType: IEventDTO, new()
        {
            // A filter maker for the events.
            private Func<Event<EventType>, NewFilterInput> filterMaker;
            
            /// <summary>
            ///   The target event.
            /// </summary>
            public readonly Event<EventType> TargetEvent;

            /// <summary>
            ///   The block the next call will start from.
            /// </summary>
            public BlockParameter FromBlock { get; private set; }

            /// <summary>
            ///   Allows instantiating the event.
            /// </summary>
            /// <param name="targetEvent">The event to track</param>
            /// <param name="makeFilter">The callback that builds the filter to use</param>
            /// <param name="fromBlock">The starting block</param>
            public EventsWorker(
                Event<EventType> targetEvent,
                Func<Event<EventType>, NewFilterInput> makeFilter = null,
                BlockParameter fromBlock = null
            )
            {
                TargetEvent = targetEvent;
                filterMaker = makeFilter ?? DefaultFilterMaker;
                FromBlock = fromBlock ?? BlockParameter.CreateEarliest();
            }

            /// <summary>
            ///   Allows instantiating the event.
            /// </summary>
            /// <param name="targetEvent">The event to track</param>
            /// <param name="makeFilter">The callback that builds the filter to use</param>
            /// <param name="fromBlock">The starting block</param>
            public EventsWorker(
                Event<EventType> targetEvent,
                Func<Event<EventType>, NewFilterInput> makeFilter,
                BigInteger fromBlock
            ) : this(targetEvent, makeFilter, new BlockParameter(new HexBigInteger(fromBlock))) {}

            // This is a default filter maker if one is not specified.
            private static NewFilterInput DefaultFilterMaker(Event<EventType> @event) {
                return @event.CreateFilterInput();
            }
            
            /// <summary>
            ///   Gets the event, and updates the FromBlock in the process.
            /// </summary>
            /// <returns>The list of retrieved events</returns>
            public async Task<List<EventLog<EventType>>> GetEvents(BlockParameter toBlock = null)
            {
                NewFilterInput filterInput = filterMaker(TargetEvent);
                filterInput.FromBlock = FromBlock;
                filterInput.ToBlock = toBlock ?? BlockParameter.CreateLatest();
                List<EventLog<EventType>> events = await TargetEvent.GetAllChangesAsync(filterInput);

                foreach (EventLog<EventType> @event in events)
                {
                    FromBlock = new BlockParameter(new HexBigInteger(@event.Log.BlockNumber.Value + 1));
                }

                if (toBlock != null)
                {
                    BlockParameter.BlockParameterType blockParamType = toBlock.ParameterType;
                    bool isExplicitBlockNumber = blockParamType == BlockParameter.BlockParameterType.blockNumber;
                    FromBlock = isExplicitBlockNumber ? new BlockParameter(new HexBigInteger(
                        toBlock.BlockNumber.Value + 1)) : FromBlock;
                }

                return events;
            }
        }
    }
}