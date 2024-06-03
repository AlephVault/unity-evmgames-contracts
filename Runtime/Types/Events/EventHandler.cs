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
        public abstract class EventHandler
        {
            /// <summary>
            ///   Contains the associated filter log.
            /// </summary>
            public readonly FilterLog FilterLog;

            /// <summary>
            ///   This method contains all the logic.
            /// </summary>
            public abstract Task Handle();

            public EventHandler(FilterLog filterLog)
            {
                FilterLog = filterLog;
            }
        }
        
        /// <summary>
        ///   A type-specific event handler.
        /// </summary>
        /// <typeparam name="T">The event type</typeparam>
        public abstract class EventHandler<T> : EventHandler
        {
            /// <summary>
            ///   The related event log.
            /// </summary>
            public readonly T Event;

            public EventHandler(EventLog<T> log) : base(log.Log)
            {
                Event = log.Event;
            }
        }
    }
}