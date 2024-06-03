using Nethereum.Contracts;

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
            ///   This method contains all the logic.
            /// </summary>
            public abstract void Handle();
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
            public readonly EventLog<T> Log;

            public EventHandler(EventLog<T> log)
            {
                Log = log;
            }
        }
    }
}