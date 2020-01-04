﻿namespace Radical.ChangeTracking
{
    using Radical.ComponentModel.ChangeTracking;
    using Radical.Validation;

    /// <summary>
    /// A base implementation of the <see cref="IChangeSetFilter"/> interface that
    /// always evaluates to true.
    /// </summary>
    public sealed class IncludeAllChangeSetFilter : IChangeSetFilter
    {
        static readonly IChangeSetFilter _instance = new IncludeAllChangeSetFilter();

        /// <summary>
        /// Gets the filter instance.
        /// </summary>
        /// <value>The filter instance.</value>
        public static IChangeSetFilter Instance
        {
            get { return _instance; }
        }

        private IncludeAllChangeSetFilter()
        {

        }

        #region IChangeSetBuilder Members

        /// <summary>
        /// Determines if the supplied IChange should be
        /// included in the built IChangeSet.
        /// </summary>
        /// <param name="change">The change to evaluate.</param>
        /// <returns></returns>
        public bool ShouldInclude(IChange change)
        {
            Ensure.That(change).Named("change").IsNotNull();

            return true;
        }

        #endregion
    }
}
