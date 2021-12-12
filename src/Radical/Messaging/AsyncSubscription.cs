using Radical.ComponentModel;
using Radical.ComponentModel.Messaging;
using Radical.Validation;
using System;

namespace Radical.Messaging
{
    interface IAsyncSubscription
    {
        object Subscriber { get; }
        object Sender { get; }

        InvocationModel InvocationModel { get; }

        SubscriptionPriority Priority { get; }

        /// <summary>
        /// Gets a reference to the callback delegate of this subscription.
        /// </summary>
        /// <returns></returns>
        Delegate GetAction();

        System.Threading.Tasks.Task<bool> ShouldInvokeAsync(Object sender, Object message);

        /// <summary>
        /// The subscriber invocation model is based on the <see cref="InvocationModel" /> property.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        System.Threading.Tasks.Task InvokeAsync(object sender, object message);

        /// <summary>
        /// The subscriber is invoked in the same thread of the dispatcher.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        System.Threading.Tasks.Task DirectInvokeAsync(object sender, object message);
    }

    class PocoAsyncSubscription<T> : IAsyncSubscription
    {
        readonly IDispatcher dispatcher;
        readonly Func<object, T, System.Threading.Tasks.Task> action;
        readonly Func<object, T, System.Threading.Tasks.Task<bool>> actionFilter;

        public PocoAsyncSubscription(object subscriber, Func<object, T, System.Threading.Tasks.Task> action, Func<object, T, System.Threading.Tasks.Task<bool>> actionFilter, InvocationModel invocationModel, IDispatcher dispatcher)
        {
            Ensure.That(subscriber).Named(() => subscriber).IsNotNull();
            Ensure.That(action).Named(() => action).IsNotNull();
            Ensure.That(actionFilter).Named(() => actionFilter).IsNotNull();
            Ensure.That(dispatcher).Named(() => dispatcher).IsNotNull();
            Ensure.That(invocationModel).Is(InvocationModel.Default);

            Subscriber = subscriber;
            this.action = action;
            this.actionFilter = actionFilter;
            Sender = null;
            InvocationModel = invocationModel;
            this.dispatcher = dispatcher;

            Priority = SubscriptionPriority.Normal;
        }

        public Delegate GetAction()
        {
            return action;
        }

        public object Subscriber { get; private set; }

        public object Sender { get; private set; }

        public InvocationModel InvocationModel { get; private set; }

        public SubscriptionPriority Priority { get; private set; }

        static System.Threading.Tasks.Task InvokeCoreAsync(IDispatcher dispatcher, Func<object, T, System.Threading.Tasks.Task> action, object sender, T message, InvocationModel type)
        {
            return action(sender, message);
        }

        public System.Threading.Tasks.Task InvokeAsync(object sender, object message)
        {
            return InvokeCoreAsync(dispatcher, action, sender, (T)message, InvocationModel.Default);
        }

        public System.Threading.Tasks.Task DirectInvokeAsync(object sender, object message)
        {
            return InvokeCoreAsync(dispatcher, action, sender, (T)message, InvocationModel.Default);
        }

        public System.Threading.Tasks.Task<bool> ShouldInvokeAsync(object sender, object message)
        {
            return actionFilter(sender, (T)message);
        }
    }

    public enum SubscriptionPriority
    {
        Highest = 3,
        High = 2,
        AboveNormal = 1,
        Normal = 0,
        BelowNormal = -1,
        Low = -2,
        Lowest = -3
    }
}