﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30128.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Radical.Resources
{


    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal static class Exceptions
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Radical.Resources.Exceptions", typeof(Exceptions).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Not allowed to add new items..
        /// </summary>
        internal static string AllowNewException
        {
            get
            {
                return ResourceManager.GetString("AllowNewException", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cannot directly access current EntityView (Unsupported Action :{0})..
        /// </summary>
        internal static string CannotAccessEntityViewException
        {
            get
            {
                return ResourceManager.GetString("CannotAccessEntityViewException", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to AllowNew is set to false, cannot call CreateNew()..
        /// </summary>
        internal static string CreateNewNotSupportedException
        {
            get
            {
                return ResourceManager.GetString("CreateNewNotSupportedException", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Supplied enum value is not defined in the supplied enum type..
        /// </summary>
        internal static string EnumValidatorNotDefinedException
        {
            get
            {
                return ResourceManager.GetString("EnumValidatorNotDefinedException", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Missing description attribute on Enum type..
        /// </summary>
        internal static string ExtractDescriptionMissingAttributeException
        {
            get
            {
                return ResourceManager.GetString("ExtractDescriptionMissingAttributeException", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Current object is read only..
        /// </summary>
        internal static string IsReadOnlyException
        {
            get
            {
                return ResourceManager.GetString("IsReadOnlyException", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to OnCreateNew cannot return a null reference.
        /// </summary>
        internal static string OnCreateNewReturnNullException
        {
            get
            {
                return ResourceManager.GetString("OnCreateNewReturnNullException", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Property not Found: {0}..
        /// </summary>
        internal static string PropertyNotFoundException
        {
            get
            {
                return ResourceManager.GetString("PropertyNotFoundException", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The suplied message type represents a type that does not inherit from IMessage.
        /// </summary>
        internal static string SubscribeToMessageAttributeInvalidMessageType
        {
            get
            {
                return ResourceManager.GetString("SubscribeToMessageAttributeInvalidMessageType", resourceCulture);
            }
        }
    }
}
