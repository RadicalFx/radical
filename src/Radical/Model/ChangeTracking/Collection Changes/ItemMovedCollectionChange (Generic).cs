﻿namespace Radical.ChangeTracking.Specialized
{
    using Radical.Collections;
    using Radical.ComponentModel.ChangeTracking;
    using Radical.Validation;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Identifies that an item in a collection has been moved.
    /// </summary>
    public class ItemMovedCollectionChange<T> : CollectionChange<ItemMovedDescriptor<T>, T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMovedCollectionChange&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="rejectCallback">The reject callback.</param>
        /// <param name="commitCallback">The commit callback.</param>
        /// <param name="description">The description.</param>
        public ItemMovedCollectionChange(Object owner, ItemMovedDescriptor<T> descriptor, RejectCallback<ItemMovedDescriptor<T>> rejectCallback, CommitCallback<ItemMovedDescriptor<T>> commitCallback, string description)
            : base(owner, descriptor, rejectCallback, commitCallback, description)
        {

        }

        /// <summary>
        /// Gets the changed entities.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Object> GetChangedEntities()
        {
            return new ReadOnlyCollection<Object>(new Object[] { this.Descriptor.Item });
        }

        /// <summary>
        /// Gets the advised action for this IChange.
        /// </summary>
        /// <param name="changedItem"></param>
        /// <returns></returns>
        /// <value>The advised action.</value>
        public override ProposedActions GetAdvisedAction(object changedItem)
        {
            Ensure.That(changedItem)
                .If(o => !Object.Equals(o, this.Descriptor.Item))
                .Then(o => { throw new ArgumentOutOfRangeException(); });

            return ProposedActions.Update | ProposedActions.Create;
        }

        /// <summary>
        /// Clones this IChange instance.
        /// </summary>
        /// <returns>A clone of this instance.</returns>
        public override IChange Clone()
        {
            return new ItemMovedCollectionChange<T>(
                this.Owner,
                this.Descriptor,
                this.RejectCallback,
                this.CommitCallback,
                this.Description);
        }
    }
}