using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace AlephVault.Unity.EVMGames.Contracts
{
    namespace Types.Events
    {
        /// <summary>
        ///   An event handler creates an arbitrary logic
        ///   to handle an incoming event.
        /// </summary>
        public abstract class EventLogHandler
        {
            /// <summary>
            ///   The underlying event log.
            /// </summary>
            public abstract FilterLog Log { get; }
            
            /// <summary>
            ///   This method contains all the logic.
            /// </summary>
            public abstract Task Handle();
        }
        
        /// <summary>
        ///   A type-specific event handler.
        /// </summary>
        /// <typeparam name="T">The event type</typeparam>
        public abstract class EventLogHandler<T> : EventLogHandler
        {
            /// <summary>
            ///   The related event log.
            /// </summary>
            public readonly EventLog<T> EventLog;

            /// <summary>
            ///   The underlying event log.
            /// </summary>
            public override FilterLog Log => EventLog.Log;

            public EventLogHandler(EventLog<T> eventLog)
            {
                EventLog = eventLog;
            }
        }
    }
}