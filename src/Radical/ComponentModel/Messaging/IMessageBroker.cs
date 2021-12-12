using System;

namespace Radical.ComponentModel.Messaging
{
    /// <summary>
    /// Defines the invocation model for the message subscription.
    /// </summary>
    public enum InvocationModel
    {
        /// <summary>
        /// The subscriber is invoked in the context and thread of the dispatcher of the message.
        /// If the message is broadcasted the subscriber is invoked in a different thread.
        /// </summary>
        Default,

        /// <summary>
        /// The subscriber is invoked in the main thread marshaling the call even 
        /// if the message has been broadcasted.
        /// </summary>
        Safe
    }

    /// <summary>
    /// A message broker is a mediator used to dispatch and 
    /// broadcast messages to all the subscribers in the system.
    /// </summary>
    public interface IMessageBroker
    {
        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe(object subscriber, object sender, Type messageType, Action<object, object> callback);

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
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe(object subscriber, object sender, Type messageType, InvocationModel invocationModel, Action<object, object> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe(object subscriber, Type messageType, Action<object, object> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe(object subscriber, Type messageType, InvocationModel invocationModel, Action<object, object> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subscriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe<T>(object subscriber, Action<object, T> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback should be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe(object subscriber, object sender, Type messageType, Func<object, object, bool> callbackFilter, Action<object, object> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback should be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe(object subscriber, object sender, Type messageType, InvocationModel invocationModel, Func<object, object, bool> callbackFilter, Action<object, object> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback should be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe(object subscriber, Type messageType, Func<object, object, bool> callbackFilter, Action<object, object> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback should be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe(object subscriber, Type messageType, InvocationModel invocationModel, Func<object, object, bool> callbackFilter, Action<object, object> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subscriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback should be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe<T>(object subscriber, Func<object, T, bool> callbackFilter, Action<object, T> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subscriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="callback">The callback.</param>
        void Subscribe<T>(object subscriber, Func<object, T, System.Threading.Tasks.Task> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subscriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback should be invoked.</param>
        /// <param name="callback">The callback.</param>
        void Subscribe<T>(object subscriber, Func<object, T, System.Threading.Tasks.Task<bool>> callbackFilter, Func<object, T, System.Threading.Tasks.Task> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <typeparam name="T">The type of message the subscriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe<T>(object subscriber, object sender, Action<object, T> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <typeparam name="T">The type of message the subscriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe<T>(object subscriber, object sender, InvocationModel invocationModel, Action<object, T> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subscriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe<T>(object subscriber, InvocationModel invocationModel, Action<object, T> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <typeparam name="T">The type of message the subscriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback should be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe<T>(object subscriber, object sender, Func<object, T, bool> callbackFilter, Action<object, T> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback only
        /// if the sender is the specified reference.
        /// </summary>
        /// <typeparam name="T">The type of message the subscriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender filter.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback should be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe<T>(object subscriber, object sender, InvocationModel invocationModel, Func<object, T, bool> callbackFilter, Action<object, T> callback);

        /// <summary>
        /// Subscribes the given subscriber to notifications of the
        /// given type of message using the supplied callback.
        /// </summary>
        /// <typeparam name="T">The type of message the subscriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="invocationModel">The invocation model.</param>
        /// <param name="callbackFilter">The filter invoked to determine if the callback should be invoked.</param>
        /// <param name="callback">The callback.</param>
        [Obsolete("The synchronous version of Subscribe is deprecated. Use the overload that accepts a Func<obect, TMessage, Task>. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Subscribe<T>(object subscriber, InvocationModel invocationModel, Func<object, T, bool> callbackFilter, Action<object, T> callback);

        /// <summary>
        /// Unsubscribe the specified subscriber from all the subscriptions.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        void Unsubscribe(object subscriber);

        /// <summary>
        /// Unsubscribe the specified subscriber from all the subscriptions to the supplied IMessage type.
        /// </summary>
        /// <typeparam name="T">The type of message the subscriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        void Unsubscribe<T>(object subscriber);

        /// <summary>
        /// Unsubscribe the specified subscriber from all the messages
        /// posted by the given sender.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender.</param>
        void Unsubscribe(object subscriber, object sender);

        /// <summary>
        /// Unsubscribe the specified subscriber from all the messages,
        /// of the given type T, posted by the given sender.
        /// </summary>
        /// <typeparam name="T">The message type filter.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="sender">The sender.</param>
        void Unsubscribe<T>(object subscriber, object sender);

        /// <summary>
        /// Unsubscribe the specified subscriber from the subscription to the supplied IMessage type.
        /// </summary>
        /// <typeparam name="T">The type of message the subscriber is interested in.</typeparam>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="callback">The callback to unsubscribe.</param>
        void Unsubscribe<T>(object subscriber, Delegate callback);

        /// <summary>
        /// Broadcasts the specified message in an asynchronous manner without
        /// waiting for the execution of the subscribers.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        [Obsolete("Broadcast is deprecated. Use BroadcastAsync. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Broadcast(object sender, object message);

        /// <summary>
        /// Broadcasts the specified message in an asynchronous manner without
        /// waiting for the execution of the subscribers.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        System.Threading.Tasks.Task BroadcastAsync(object sender, object message);

        /// <summary>
        /// Dispatches the specified message in a synchronous manner waiting for
        /// the execution of all the subscribers.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message to dispatch.</param>
        [Obsolete("Dispatch is deprecated. Use BroadcastAsync. It will be treated as an error in v3 and removed in v4.", error: false)]
        void Dispatch(object sender, object message);
    }
}