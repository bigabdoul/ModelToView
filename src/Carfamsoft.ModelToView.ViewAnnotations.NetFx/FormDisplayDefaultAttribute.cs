using System;
using System.Resources;

namespace Carfamsoft.ModelToView.ViewAnnotations
{
    /// <summary>
    /// Represents a custom attributes that encapsulates default 
    /// values for an instance of the <see cref="FormDisplayAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FormDisplayDefaultAttribute : FormAttributeBase
    {
        internal static readonly FormDisplayDefaultAttribute Empty = new FormDisplayDefaultAttribute();

        private ResourceManager _resourceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormDisplayDefaultAttribute"/> class.
        /// </summary>
        public FormDisplayDefaultAttribute()
        {
            InputCssClass = "form-control";
        }

        /// <summary>
        /// Gets or sets the CSS class (e.g. row) for a section element.
        /// The default value is "row".
        /// </summary>
        public string GroupCssClass { get; set; } = "row";

        /// <summary>
        /// Gets or sets the CSS class (e.g. col) for an HTML element wrapped around an input.
        /// The default value is "col".
        /// </summary>
        public string ColumnCssClass { get; set; } = "col";

        /// <summary>
        /// Indicates whether to display the name of a group.
        /// </summary>
        public bool ShowGroupName { get; set; }

        /// <summary>
        /// Indicates whether rendering custom inputs is disabled, enabled or determined
        /// by the default value. The default is <see cref="CustomRenderMode.Enabled"/>.
        /// </summary>
        public CustomRenderMode CustomRenderMode { get; set; } = CustomRenderMode.Enabled;

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

        /// <summary>
        /// Creates a new instance of the <see cref="FormDisplayAttribute"/> from matching 
        /// properties of the current <see cref="FormDisplayDefaultAttribute"/> instance.
        /// </summary>
        /// <returns></returns>
        internal FormDisplayAttribute CreateDefault()
        {
            return new FormDisplayAttribute
            {
                ColumnCssClass = ColumnCssClass,
                InputCssClass = InputCssClass,
                GroupName = string.Empty,
                CustomRenderMode = CustomRenderMode,
            };
        }
    }
}
