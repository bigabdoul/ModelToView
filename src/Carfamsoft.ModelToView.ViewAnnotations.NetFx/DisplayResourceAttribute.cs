using System;
using System.Resources;

namespace Carfamsoft.ModelToView.ViewAnnotations
{
    /// <summary>
    /// Provides a general-purpose attribute that lets you specify the resource manager type for localizable strings for properties of entity partial classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Obsolete("Use Carfamsoft.ModelToView.ViewAnnotations.FormDisplayDefaultAttribute")]
    public class DisplayResourceAttribute : Attribute
    {
        private ResourceManager _resourceManager;

        /// <summary>
        /// Gets or sets the type that contains the resources for all properties of the decorated entity class.
        /// </summary>
        public Type ResourceType { get; set; }

        /// <summary>
        /// Returns an initialized instance of the <see cref="ResourceManager"/> 
        /// class for the <see cref="ResourceType"/> property.
        /// </summary>
        /// <returns></returns>
        public ResourceManager GetResourceManager()
        {
            if (ResourceType == null) return null;
            if (_resourceManager == null)
            {
                _resourceManager = new ResourceManager(ResourceType);
            }
            return _resourceManager;
        }
    }
}
