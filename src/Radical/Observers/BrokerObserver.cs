using Radical.ComponentModel.Messaging;
using Radical.Validation;
using System;
using System.Threading.Tasks;

namespace Radical.Observers
{
    /// <summary>
    /// A static entry to simplify the creation a <see cref="MessageBrokerMonitor"/>.
    /// </summary>
    public static class BrokerObserver
    {
        /// <summary>
        /// Creates a new <see cref="MessageBrokerMonitor"/> that monitors 
        /// the specified message broker.
        /// </summary>
        /// <param name="broker">The message broker.</param>
        /// <returns>The new <see cref="MessageBrokerMonitor"/>.</returns>
        public static MessageBrokerMonitor Using(IMessageBroker broker)
        {
            return new MessageBrokerMonitor(broker);
        }
    }

    /// <summary>
    /// An observer to monitor messages that are handled by a given message broker.
    /// </summary>
    public class MessageBrokerMonitor :
        AbstractMonitor<IMessageBroker>
    {
        readonly IMessageBroker broker;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBrokerMonitor"/> class.
        /// </summary>
        /// <param name="broker">The message broker.</param>
        public MessageBrokerMonitor(IMessageBroker broker)
            : base(broker)
        {
            Ensure.That(broker).Named("broker").IsNotNull();
            this.broker = broker;
        }

        /// <summary>
        /// Waits for the specified message type and raise the Changed event whenever the
        /// specified message is dispatched or broadcast.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <returns>This monitor instance.</returns>
        public MessageBrokerMonitor WaitingFor<TMessage>() where TMessage : class
        {
            return WaitingFor<TMessage>(m => Task.FromResult(true));
        }

        /// <summary>
        /// Waits for the specified message type and raise the Changed event whenever the
        /// specified message is dispatched or broadcast.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="invocationModel">The invocation model.</param>
        /// <returns>This monitor instance.</returns>
        public MessageBrokerMonitor WaitingFor<TMessage>(InvocationModel invocationModel) where TMessage : class
        {
            return WaitingFor<TMessage>(m => true, invocationModel);
        }

        /// <summary>
        /// Waits for the specified message type and raise the Changed event 
        /// if the supplied condition is satisfied by the dispatched or broadcast message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="filter">The filter condition.</param>
        /// <returns>This monitor instance.</returns>
        [Obsolete("The synchronous version of WaitingFor is deprecated. Use the overload that accepts a Func<TMessage, Task<bool>>. It will be treated as an error in v3 and removed in v4.", error: false)]
        public MessageBrokerMonitor WaitingFor<TMessage>(Func<TMessage, bool> filter) where TMessage : class
        {
            broker.Subscribe<TMessage>(this, (sender, msg) =>
           {
               if (filter(msg))
               {
                   OnChanged();
               }
           });

            return this;
        }

        /// <summary>
        /// Waits for the specified message type and raise the Changed event 
        /// if the supplied condition is satisfied by the dispatched or broadcast message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="filter">The filter condition.</param>
        /// <returns>This monitor instance.</returns>
        public MessageBrokerMonitor WaitingFor<TMessage>(Func<TMessage, Task<bool>> filter) where TMessage : class
        {
            broker.Subscribe<TMessage>(this, async (sender, msg) =>
            {
                if (await filter(msg).ConfigureAwait(false))
                {
                    OnChanged();
                }
            });

            return this;
        }

        /// <summary>
        /// Waits for the specified message type and raise the Changed event 
        /// if the supplied condition is satisfied by the dispatched or broadcast message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="filter">The filter condition.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <returns>This monitor instance.</returns>
        [Obsolete("The synchronous version of WaitingFor is deprecated. Use the overload that accepts a Func<TMessage, Task<bool>>. It will be treated as an error in v3 and removed in v4.", error: false)]
        public MessageBrokerMonitor WaitingFor<TMessage>(Func<TMessage, bool> filter, InvocationModel invocationModel) where TMessage : class
        {
            broker.Subscribe<TMessage>(this, invocationModel, (sender, msg) =>
            {
                if (filter(msg))
                {
                    OnChanged();
                }
            });

            return this;
        }

        /// <summary>
        /// Called in order to allow inheritors to stop the monitoring operations.
        /// </summary>
        /// <param name="targetDisposed"><c>True</c> if this call is subsequent to the Dispose of the monitored instance.</param>
        protected override void OnStopMonitoring(bool targetDisposed)
        {
            broker.Unsubscribe(this);
        }
    }
}
