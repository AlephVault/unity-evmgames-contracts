using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Types.Events
    {
        /// <summary>
        ///   Processes the incoming events.
        /// </summary>
        public abstract class EventsProcessor
        {
            /// <summary>
            ///   Gets handlers related to events until some related block.
            /// </summary>
            /// <param name="toBlock">The target block</param>
            /// <returns>The list of handlers</returns>
            public abstract Task<List<EventHandler>> GetHandlers(BlockParameter toBlock = null);
        }

        /// <summary>
        ///   Processes the incoming events of certain type.
        /// </summary>
        /// <typeparam name="EventType">The event type</typeparam>
        /// <typeparam name="EventHandlerType">The handler type</typeparam>
        public abstract class EventsProcessor<EventType, EventHandlerType> : EventsProcessor
            where EventHandlerType : EventHandler<EventType>
            where EventType : IEventDTO, new()
        {
            private Func<EventLog<EventType>, EventHandlerType> _wrapper;
            private EventsWorker<EventType> _worker;
            
            public EventsProcessor(
                EventsWorker<EventType> worker,
                Func<EventLog<EventType>, EventHandlerType> wrapper
            ) {
                _wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
                _worker = worker ?? throw new ArgumentNullException(nameof(worker));
            }
            
            private List<EventHandler> GetHandlers(List<EventLog<EventType>> eventLogs) {
                return (from log in eventLogs select _wrapper(log) as EventHandler).ToList();
            }

            /// <summary>
            ///   Gets handlers related to events until some related block.
            /// </summary>
            /// <param name="toBlock">The target block</param>
            /// <returns>The list of handlers</returns>
            public override async Task<List<EventHandler>> GetHandlers(BlockParameter toBlock = null)
            {
                return GetHandlers(await _worker.GetEvents(toBlock));
            }
        }
    }
}