using Radical.ComponentModel;
using Radical.ComponentModel.Messaging;
using Radical.Diagnostics;
using Radical.Linq;
using Radical.Reflection;
using Radical.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Radical.Messaging
{
    /// <summary>
    /// A message broker is a mediator used to dispatch and 
    /// broadcast messages to all the subscribers in the system.
    /// </summary>
    public class MessageBroker : IMessageBroker
    {
        class SubscriptionsContainer
        {
            public SubscriptionsContainer(Type messageType)
            {
                MessageType = messageType;
                Subscriptions = new List<ISubscription>();
            }

            public Type MessageType { get; private set; }

            public List<ISubscription> Subscriptions { get; private set; }
        }

        static readonly TraceSource logger = new TraceSource(typeof(MessageBroker).FullName);
        
        readonly IDispatcher dispatcher;

        readonly List<SubscriptionsContainer> msgSubsIndex = null;
        readonly ReaderWriterLockSlim msgSubsIndexLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBroker"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        public MessageBroker(IDispatcher dispatcher)
        {
            Ensure.That(dispatcher).Named("dispatcher").IsNotNull();

            this.dispatcher = dispatcher;
            msgSubsIndex = new List<SubscriptionsContainer>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBroker"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="factory">The factory.</param>
        [Obsolete("Constructing a message broker with a task factory is deprecated.", error: true)]
        public MessageBroker(IDispatcher dispatcher, TaskFactory factory)
        {
            throw new NotSupportedException();
        }

        void SubscribeCore(Type messageType, ISubscription subscription)
        {
            msgSubsIndexLock.EnterUpgradeableReadLock();
            try
            {
                if (msgSubsIndex.Any(sc => sc.MessageType == messageType))
                {
                    var allMessageSubscriptions = msgSubsIndex.Single(sc => sc.MessageType == messageType).Subscriptions;
                    msgSubsIndexLock.EnterWriteLock();
                    try
                    {
                        allMessageSubscriptions.Add(subscription);
                    }
                    finally
                    {
                        msgSubsIndexLock.ExitWriteLock();
                    }
                }
                else
                {
                    msgSubsIndexLock.EnterWriteLock();
                    try
                    {
                        var sc = new SubscriptionsContainer(messageType);
                        sc.Subscriptions.Add(subscription);
                        msgSubsIndex.Add(sc);
                    }
                    finally
                    {
                        msgSubsIndexLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                msgSubsIndexLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe<T>(object subscriber, Action<object, T> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will removed in v4.", error: true)]
        public void Subscribe<T>(object subscriber, object sender, Action<object, T> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe(object subscriber, Type messageType, Action<object, object> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe(object subscriber, object sender, Type messageType, Action<object, object> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe<T>(object subscriber, InvocationModel invocationModel, Action<object, T> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe(object subscriber, Type messageType, InvocationModel invocationModel, Action<object, object> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe(object subscriber, object sender, Type messageType, InvocationModel invocationModel, Action<object, object> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe<T>(object subscriber, object sender, InvocationModel invocationModel, Action<object, T> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Unsubscribes the specified subscriber from all the subcscriptions.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        public void Unsubscribe(object subscriber)
        {
            Ensure.That(subscriber).Named(() => subscriber).IsNotNull();

            msgSubsIndexLock.EnterUpgradeableReadLock();
            try
            {
                foreach (var subscription in msgSubsIndex)
                {
                    var count = subscription.Subscriptions.Count;
                    for (var k = count; k > 0; k--)
                    {
                        var sub = subscription.Subscriptions[k - 1];
                        if (sub.Subscriber == subscriber)
                        {
                            msgSubsIndexLock.EnterWriteLock();
                            try
                            {
                                subscription.Subscriptions.Remove(sub);
                            }
                            finally
                            {
                                msgSubsIndexLock.ExitWriteLock();
                            }
                        }
                    }
                }

                msgSubsIndex.Where(msgSubscriptions => msgSubscriptions.Subscriptions.Count == 0)
                    .ToList()
                    .ForEach(kvp =>
                   {
                       msgSubsIndexLock.EnterWriteLock();
                       try
                       {
                           msgSubsIndex.Remove(kvp);
                       }
                       finally
                       {
                           msgSubsIndexLock.ExitWriteLock();
                       }
                   });
            }
            finally
            {
                msgSubsIndexLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Unsubscribes the specified subscriber from all the messages
        /// posted by the given sender.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender.</param>
        public void Unsubscribe(object subscriber, object sender)
        {
            Ensure.That(subscriber).Named(() => subscriber).IsNotNull();
            Ensure.That(sender).Named(() => sender).IsNotNull();

            msgSubsIndexLock.EnterUpgradeableReadLock();
            try
            {
                msgSubsIndex.Where(msgSubscriptions =>
                {
                    return msgSubscriptions.Subscriptions.Any(subscription =>
                    {
                        return Equals(subscription.Subscriber, subscriber)
                            && Equals(subscription.Sender, sender);
                    });
                })
                .ToList()
                .ForEach(kvp =>
                {
                    msgSubsIndexLock.EnterWriteLock();
                    try
                    {
                        msgSubsIndex.Remove(kvp);
                    }
                    finally
                    {
                        msgSubsIndexLock.ExitWriteLock();
                    }
                });
            }
            finally
            {
                msgSubsIndexLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Unsubscribes the specified subscriber from all the subcscriptions to the supplied IMessage type.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        public void Unsubscribe<T>(object subscriber)
        {
            Ensure.That(subscriber).Named("subscriber").IsNotNull();

            msgSubsIndexLock.EnterUpgradeableReadLock();
            try
            {
                if (msgSubsIndex.Any(sc => sc.MessageType == typeof(T)))
                {
                    var allMessageSubscriptions = msgSubsIndex.Single(sc => sc.MessageType == typeof(T)).Subscriptions;
                    allMessageSubscriptions.Where(subscription =>
                   {
                       return Equals(subscriber, subscription.Subscriber);
                   })
                    .ToList()
                    .ForEach(subscription =>
                   {
                       msgSubsIndexLock.EnterWriteLock();
                       try
                       {
                           allMessageSubscriptions.Remove(subscription);
                       }
                       finally
                       {
                           msgSubsIndexLock.ExitWriteLock();
                       }
                   });
                }
            }
            finally
            {
                msgSubsIndexLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Unsubscribes the specified subscriber from all the messages,
        /// of the given type T, posted by the given sender.
        /// </summary>
        /// <typeparam name="T">The message type filter.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender.</param>
        public void Unsubscribe<T>(object subscriber, object sender)
        {
            Ensure.That(subscriber).Named("subscriber").IsNotNull();
            Ensure.That(sender).Named("sender").IsNotNull();

            msgSubsIndexLock.EnterUpgradeableReadLock();
            try
            {
                if (msgSubsIndex.Any(sc => sc.MessageType == typeof(T)))
                {
                    var allMessageSubscriptions = msgSubsIndex.Single(sc => sc.MessageType == typeof(T)).Subscriptions;
                    allMessageSubscriptions.Where(subscription =>
                   {
                       return Equals(subscriber, subscription.Subscriber)
                              && Equals(sender, subscription.Sender);
                   })
                    .ToList()
                    .ForEach(subscription =>
                   {
                       msgSubsIndexLock.EnterWriteLock();
                       try
                       {
                           allMessageSubscriptions.Remove(subscription);
                       }
                       finally
                       {
                           msgSubsIndexLock.ExitWriteLock();
                       }

                   });
                }
            }
            finally
            {
                msgSubsIndexLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Unsubscribes the specified subscriber from the subcscription to the supplied IMessage type.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="callback">The callback to unsubscribe.</param>
        public void Unsubscribe<T>(object subscriber, Delegate callback)
        {
            Ensure.That(subscriber).Named("subscriber").IsNotNull();
            Ensure.That(callback).Named("callback").IsNotNull();

            msgSubsIndexLock.EnterUpgradeableReadLock();
            try
            {
                if (msgSubsIndex.Any(sc => sc.MessageType == typeof(T)))
                {
                    var allMessageSubscriptions = msgSubsIndex.Single(sc => sc.MessageType == typeof(T)).Subscriptions;
                    allMessageSubscriptions.Where(subscription =>
                   {
                       return Equals(subscriber, subscription.Subscriber)
                              && Equals(callback, subscription.GetAction());
                   })
                    .ToList()
                    .ForEach(subscription =>
                   {
                       msgSubsIndexLock.EnterWriteLock();
                       try
                       {
                           allMessageSubscriptions.Remove(subscription);
                       }
                       finally
                       {
                           msgSubsIndexLock.ExitWriteLock();
                       }
                   });
                }
            }
            finally
            {
                msgSubsIndexLock.ExitUpgradeableReadLock();
            }
        }

        IEnumerable<ISubscription> GetSubscriptionsFor(Type messageType, object sender)
        {
            msgSubsIndexLock.EnterReadLock();
            try
            {
                var subscriptions = msgSubsIndex
                                        .Where(kvp => messageType.Is(kvp.MessageType))
                                        .SelectMany(kvp => kvp.Subscriptions);

                var effectiveSubscribers = subscriptions.Where(s => s.Sender == null || s.Sender == sender)
                                                        .OrderByDescending(s => s.Priority)
                                                        .AsReadOnly();

                return effectiveSubscribers;
            }
            finally
            {
                msgSubsIndexLock.ExitReadLock();
            }
        }

        [Obsolete("Dispatch is deprecated. Use BroadcastAsync. It will be removed in v4.", error: true)]
        public void Dispatch(object sender, object message)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Broadcasts the specified message in an asynchronus manner without
        /// waiting for the execution of the subscribers.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        [Obsolete("Broadcast is deprecated. Use BroadcastAsync. It will be removed in v4.", error: true)]
        public void Broadcast(object sender, object message)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Broadcasts the specified message in an asynchronus manner without
        /// waiting for the execution of the subscribers.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        public Task BroadcastAsync(object sender, object message)
        {
            Ensure.That(message).Named(() => message).IsNotNull();
            Ensure.That(sender).Named(() => sender).IsNotNull();

            if (message is IRequireToBeValid requireToBeValid)
            {
                requireToBeValid.Validate();
            }

            var subscriptions = GetSubscriptionsFor(message.GetType(), sender);

            var tasks = new List<Task>();
            if (subscriptions.Any())
            {
                subscriptions
                    .Where(sub => sub.ShouldInvoke(sender, message))
                    .ForEach(sub =>
                    {
                        var _as = sub as IAsyncSubscription;
                        if (_as != null)
                        {
                            tasks.Add(_as.InvokeAsync(sender, message));
                        }
                        else
                        {
                            tasks.Add(factory.StartNew(() =>
                            {
                                sub.Invoke(sender, message);
                            }, TaskCreationOptions.LongRunning));
                        }
                    });

            }

            return Task.WhenAll(tasks.ToArray());
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="callback">The callback.</param>
        public void Subscribe<T>(object subscriber, Func<object, T, Task> callback)
        {
            Subscribe(subscriber, (s, msg) => Task.FromResult(true), callback);
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback shopuld be invoked.</param>
        /// <param name="callback">The callback.</param>
        public void Subscribe<T>(object subscriber, Func<object, T, Task<bool>> callbackFilter, Func<object, T, Task> callback)
        {
            Ensure.That(subscriber).Named("subscriber").IsNotNull();
            Ensure.That(callbackFilter).Named("callbackFilter").IsNotNull();
            Ensure.That(callback).Named("callback").IsNotNull();

            var subscription = new PocoAsyncSubscription<T>(subscriber, callback, callbackFilter, InvocationModel.Default, dispatcher);

            SubscribeCore(typeof(T), subscription);
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback shopuld be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe(object subscriber, object sender, Type messageType, Func<object, object, bool> callbackFilter, Action<object, object> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback shopuld be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe(object subscriber, object sender, Type messageType, InvocationModel invocationModel, Func<object, object, bool> callbackFilter, Action<object, object> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback shopuld be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe(object subscriber, Type messageType, Func<object, object, bool> callbackFilter, Action<object, object> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback shopuld be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe(object subscriber, Type messageType, InvocationModel invocationModel, Func<object, object, bool> callbackFilter, Action<object, object> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback shopuld be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe<T>(object subscriber, Func<object, T, bool> callbackFilter, Action<object, T> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback shopuld be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe<T>(object subscriber, object sender, Func<object, T, bool> callbackFilter, Action<object, T> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback shopuld be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe<T>(object subscriber, object sender, InvocationModel invocationModel, Func<object, T, bool> callbackFilter, Action<object, T> callback)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback shopuld be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be removed in v4.", error: true)]
        public void Subscribe<T>(object subscriber, InvocationModel invocationModel, Func<object, T, bool> callbackFilter, Action<object, T> callback)
        {
            throw new NotSupportedException();
        }
    }
}