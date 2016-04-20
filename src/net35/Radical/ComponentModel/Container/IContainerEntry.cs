﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Topics.Radical.ComponentModel
{
    /// <summary>
    /// Defines a container entry.
    /// </summary>
    public interface IContainerEntry
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        String Key { get; }

        /// <summary>
        /// Gets the component type.
        /// </summary>
        /// <value>The component type.</value>
        Type Component { get; }

        /// <summary>
        /// Gets the service types.
        /// </summary>
        /// <value>
        /// The service types.
        /// </value>
        IEnumerable<Type> Services { get; }

        /// <summary>
        /// Gets the factory used to build up a concrete type.
        /// </summary>
        /// <value>The factory.</value>
        Delegate Factory { get; }

        /// <summary>
        /// Gets the lifestyle of this component.
        /// </summary>
        /// <value>The lifestyle.</value>
        Lifestyle Lifestyle { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        IDictionary<String, Object> Parameters { get; }

        /// <summary>
        /// Gets an indication is this component is overridable.
        /// </summary>
        /// <value>
        /// An indication is this component is overridable.
        /// </value>
        Boolean IsOverridable { get; }
    }
}
