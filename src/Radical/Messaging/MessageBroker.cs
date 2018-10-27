﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Radical.ComponentModel;
using Radical.ComponentModel.Messaging;
using System.Reflection;
using Radical.Validation;
using Radical.Linq;
using Radical.Reflection;
using System.Diagnostics;
using Radical.Conversions;
using Radical.Diagnostics;
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
                this.MessageType = messageType;
                this.Subscriptions = new List<ISubscription>();
            }

            public Type MessageType { get; private set; }

            public List<ISubscription> Subscriptions { get; private set; }
        }

        static readonly TraceSource logger = new TraceSource(typeof(MessageBroker).FullName);
        TaskFactory factory = null;

        readonly IDispatcher dispatcher;

        /// <summary>
        /// A dictionary, of all the subscriptions, whose key is the message type
        /// and whose value is the list of Subscriptions for that message type
        /// </summary>
        readonly List<SubscriptionsContainer> msgSubsIndex = null;
        readonly ReaderWriterLockSlim msgSubsIndexLock = new ReaderWriterLockSlim();

        //Dictionary<String, IScopedMessageBroker> topics = new Dictionary<string, IScopedMessageBroker>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBroker"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        public MessageBroker(IDispatcher dispatcher)
        {
            Ensure.That(dispatcher).Named("dispatcher").IsNotNull();

            this.dispatcher = dispatcher;
            this.msgSubsIndex = new List<SubscriptionsContainer>();

            this.factory = new TaskFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBroker"/> class.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="factory">The factory.</param>
        public MessageBroker(IDispatcher dispatcher, TaskFactory factory)
        {
            Ensure.That(dispatcher).Named("dispatcher").IsNotNull();
            Ensure.That(factory).Named(() => factory).IsNotNull();

            this.dispatcher = dispatcher;
            this.factory = factory;
            this.msgSubsIndex = new List<SubscriptionsContainer>();
        }

        void SubscribeCore(Type messageType, ISubscription subscription)
        {
            msgSubsIndexLock.EnterUpgradeableReadLock();
            try
            {
                if (this.msgSubsIndex.Any(sc => sc.MessageType == messageType))
                {
                    var allMessageSubscriptions = this.msgSubsIndex.Single(sc => sc.MessageType == messageType).Subscriptions;
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
                        this.msgSubsIndex.Add(sc);
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
        public void Subscribe<T>(object subscriber, Action<Object, T> callback)
        {
            this.Subscribe<T>(subscriber, InvocationModel.Default, callback);
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
        public void Subscribe<T>(object subscriber, object sender, Action<Object, T> callback)
        {
            this.Subscribe<T>(subscriber, sender, InvocationModel.Default, callback);
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="callback">The callback.</param>
        public void Subscribe(object subscriber, Type messageType, Action<Object, Object> callback)
        {
            this.Subscribe(subscriber, messageType, InvocationModel.Default, callback);
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
        public void Subscribe(object subscriber, object sender, Type messageType, Action<Object, Object> callback)
        {
            this.Subscribe(subscriber, sender, messageType, InvocationModel.Default, callback);
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callback">The callback.</param>
        public void Subscribe<T>(object subscriber, InvocationModel invocationModel, Action<Object, T> callback)
        {
            this.Subscribe<T>(subscriber, invocationModel, (s, msg) => true, callback);
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callback">The callback.</param>
        public void Subscribe(object subscriber, Type messageType, InvocationModel invocationModel, Action<Object, Object> callback)
        {
            this.Subscribe(subscriber, messageType, invocationModel, (s, msg) => true, callback);
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
        public void Subscribe(object subscriber, object sender, Type messageType, InvocationModel invocationModel, Action<Object, Object> callback)
        {
            this.Subscribe(subscriber, sender, messageType, invocationModel, (s, msg) => true, callback);
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
        public void Subscribe<T>(object subscriber, object sender, InvocationModel invocationModel, Action<Object, T> callback)
        {
            this.Subscribe<T>(subscriber, sender, invocationModel, (s, msg) => true, callback);
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
                foreach (var subscription in this.msgSubsIndex)
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

                this.msgSubsIndex.Where(msgSubscriptions => msgSubscriptions.Subscriptions.Count == 0)
                    .ToList()
                    .ForEach(kvp =>
                   {
                       msgSubsIndexLock.EnterWriteLock();
                       try
                       {
                           this.msgSubsIndex.Remove(kvp);
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
        public void Unsubscribe(Object subscriber, Object sender)
        {
            Ensure.That(subscriber).Named(() => subscriber).IsNotNull();
            Ensure.That(sender).Named(() => sender).IsNotNull();

            msgSubsIndexLock.EnterUpgradeableReadLock();
            try
            {
                this.msgSubsIndex.Where(msgSubscriptions =>
               {
                   return msgSubscriptions.Subscriptions.Where(subscription =>
                   {
                       return Object.Equals(subscription.Subscriber, subscriber)
                              && Object.Equals(subscription.Sender, sender);
                   })
                   .Any();
               })
                .ToList()
                .ForEach(kvp =>
               {
                   msgSubsIndexLock.EnterWriteLock();
                   try
                   {
                       this.msgSubsIndex.Remove(kvp);
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
                if (this.msgSubsIndex.Any(sc => sc.MessageType == typeof(T)))
                {
                    var allMessageSubscriptions = this.msgSubsIndex.Single(sc => sc.MessageType == typeof(T)).Subscriptions;
                    allMessageSubscriptions.Where(subscription =>
                   {
                       return Object.Equals(subscriber, subscription.Subscriber);
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
                if (this.msgSubsIndex.Any(sc => sc.MessageType == typeof(T)))
                {
                    var allMessageSubscriptions = this.msgSubsIndex.Single(sc => sc.MessageType == typeof(T)).Subscriptions;
                    allMessageSubscriptions.Where(subscription =>
                   {
                       return Object.Equals(subscriber, subscription.Subscriber)
                              && Object.Equals(sender, subscription.Sender);
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
                if (this.msgSubsIndex.Any(sc => sc.MessageType == typeof(T)))
                {
                    var allMessageSubscriptions = this.msgSubsIndex.Single(sc => sc.MessageType == typeof(T)).Subscriptions;
                    allMessageSubscriptions.Where(subscription =>
                   {
                       return Object.Equals(subscriber, subscription.Subscriber)
                              && Object.Equals(callback, subscription.GetAction());
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

        IEnumerable<ISubscription> GetSubscriptionsFor(Type messageType, Object sender)
        {
            msgSubsIndexLock.EnterReadLock();
            try
            {
                var subscriptions = this.msgSubsIndex
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

        public void Dispatch(Object sender, Object message)
        {
            Ensure.That(sender).Named(() => sender).IsNotNull();
            Ensure.That(message).Named(() => message).IsNotNull();

            message.As<IRequireToBeValid>(m => m.Validate());

            var messageType = message.GetType();
            var subscriptions = this.GetSubscriptionsFor(messageType, sender);
            var anySubscription = subscriptions.Any();

            if (!anySubscription)
            {
                logger.Warning("No Subscribers for the given message type: {0}", messageType.ToString("SN"));
            }
            else
            {
                subscriptions
                    .Where(sub => sub.ShouldInvoke(sender, message))
                    .ForEach(subscription => subscription.DirectInvoke(sender, message));
            }
        }

        /// <summary>
        /// Broadcasts the specified message in an asynchronus manner without
        /// waiting for the execution of the subscribers.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        public void Broadcast(Object sender, Object message)
        {
            Ensure.That(message).Named(() => message).IsNotNull();
            Ensure.That(sender).Named(() => sender).IsNotNull();

            message.As<IRequireToBeValid>(m => m.Validate());

            var subscriptions = this.GetSubscriptionsFor(message.GetType(), sender);

            if (subscriptions.Any())
            {

                subscriptions.Where(sub => sub.ShouldInvoke(sender, message))
                    .ForEach(sub =>
                    {
                        this.factory.StartNew(() =>
                        {
                            sub.Invoke(sender, message);
                        });
                    });
            }
        }

        /// <summary>
        /// Broadcasts the specified message in an asynchronus manner without
        /// waiting for the execution of the subscribers.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        public Task BroadcastAsync(Object sender, Object message)
        {
            Ensure.That(message).Named(() => message).IsNotNull();
            Ensure.That(sender).Named(() => sender).IsNotNull();

            message.As<IRequireToBeValid>(m => m.Validate());

            var subscriptions = this.GetSubscriptionsFor(message.GetType(), sender);

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
                            tasks.Add(this.factory.StartNew(() =>
                            {
                                sub.Invoke(sender, message);
                            }));
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
            this.Subscribe(subscriber, (s, msg) => Task.FromResult(true), callback);
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

            var subscription = new PocoAsyncSubscription<T>(subscriber, callback, callbackFilter, InvocationModel.Default, this.dispatcher);

            this.SubscribeCore(typeof(T), subscription);
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
        public void Subscribe(object subscriber, object sender, Type messageType, Func<object, object, bool> callbackFilter, Action<object, object> callback)
        {
            this.Subscribe(subscriber, sender, messageType, InvocationModel.Default, callbackFilter, callback);
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
        public void Subscribe(object subscriber, object sender, Type messageType, InvocationModel invocationModel, Func<object, object, bool> callbackFilter, Action<object, object> callback)
        {
            Ensure.That(subscriber).Named(() => subscriber).IsNotNull();
            Ensure.That(sender).Named(() => sender).IsNotNull();
            Ensure.That(messageType).Named(() => messageType).IsNotNull();
            Ensure.That(callback).Named(() => callback).IsNotNull();
            Ensure.That(callbackFilter).Named(() => callbackFilter).IsNotNull();

            var subscription = new PocoSubscription(subscriber, sender, callback, callbackFilter, invocationModel, this.dispatcher);

            this.SubscribeCore(messageType, subscription);
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback shopuld be invoked.</param>
        /// <param name="callback">The callback.</param>
        public void Subscribe(object subscriber, Type messageType, Func<object, object, bool> callbackFilter, Action<object, object> callback)
        {
            this.Subscribe(subscriber, messageType, InvocationModel.Default, callbackFilter, callback);
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
        public void Subscribe(object subscriber, Type messageType, InvocationModel invocationModel, Func<object, object, bool> callbackFilter, Action<object, object> callback)
        {
            Ensure.That(subscriber).Named(() => subscriber).IsNotNull();
            Ensure.That(messageType).Named(() => messageType).IsNotNull();
            Ensure.That(callbackFilter).Named(() => callbackFilter).IsNotNull();
            Ensure.That(callback).Named(() => callback).IsNotNull();

            var subscription = new PocoSubscription(subscriber, callback, callbackFilter, invocationModel, this.dispatcher);

            this.SubscribeCore(messageType, subscription);
        }

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subecriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback shopuld be invoked.</param>
        /// <param name="callback">The callback.</param>
        public void Subscribe<T>(object subscriber, Func<object, T, bool> callbackFilter, Action<object, T> callback)
        {
            this.Subscribe<T>(subscriber, InvocationModel.Default, callbackFilter, callback);
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
        public void Subscribe<T>(object subscriber, object sender, Func<object, T, bool> callbackFilter, Action<object, T> callback)
        {
            this.Subscribe<T>(subscriber, sender, InvocationModel.Default, callbackFilter, callback);
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
        public void Subscribe<T>(object subscriber, object sender, InvocationModel invocationModel, Func<object, T, bool> callbackFilter, Action<object, T> callback)
        {
            Ensure.That(subscriber).Named(() => subscriber).IsNotNull();
            Ensure.That(sender).Named(() => sender).IsNotNull();
            Ensure.That(callback).Named(() => callback).IsNotNull();
            Ensure.That(callbackFilter).Named(() => callbackFilter).IsNotNull();

            var subscription = new PocoSubscription<T>(subscriber, sender, callback, callbackFilter, invocationModel, this.dispatcher);

            this.SubscribeCore(typeof(T), subscription);
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
        public void Subscribe<T>(object subscriber, InvocationModel invocationModel, Func<object, T, bool> callbackFilter, Action<object, T> callback)
        {
            Ensure.That(subscriber).Named(() => subscriber).IsNotNull();
            Ensure.That(callback).Named(() => callback).IsNotNull();
            Ensure.That(callbackFilter).Named(() => callbackFilter).IsNotNull();

            var subscription = new PocoSubscription<T>(subscriber, callback, callbackFilter, invocationModel, this.dispatcher);

            this.SubscribeCore(typeof(T), subscription);
        }
    }
}