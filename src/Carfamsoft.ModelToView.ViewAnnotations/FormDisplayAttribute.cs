using Carfamsoft.ModelToView.Shared;
using Carfamsoft.ModelToView.Shared.Extensions;
using System;
using System.Reflection;
using System.Resources;

namespace Carfamsoft.ModelToView.ViewAnnotations
{
    /// <summary>
    /// Represents a custom attribute that specifies layout for a form input.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FormDisplayAttribute : FormAttributeBase
    {
        /// <summary>
        /// Provides a value to use for default form display options.
        /// </summary>
        public static readonly FormDisplayAttribute Empty = new FormDisplayAttribute
        {
            GroupName = string.Empty,
            ColumnCssClass = FormDisplayDefaultAttribute.Empty.ColumnCssClass,
            InputCssClass = FormDisplayDefaultAttribute.Empty.InputCssClass,
            CustomRenderMode = FormDisplayDefaultAttribute.Empty.CustomRenderMode,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="FormDisplayAttribute"/> class.
        /// </summary>
        public FormDisplayAttribute()
        {
            if (InputCssClass == "form-control" && (IsInputCheckbox || IsInputRadio))
                InputCssClass = null;
        }

        /// <summary>
        /// Gets or sets a value that is used to group fields in the UI.
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value that is used for display in the UI.
        /// Set to an empty string (value of <see cref="string.Empty"/>, not null) to omit the label.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value that is used for the grid column label.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        ///  Gets or sets a value that is used to display a description in the UI.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the type that contains the resources for the <see cref="ShortName"/>,
        /// <see cref="Name"/>, <see cref="FormAttributeBase.Prompt"/>, and <see cref="Description"/> properties.
        /// </summary>
        public Type ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the order weight of the column.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the CSS class (e.g. col-md-4) used for an element (column)
        /// wrapped around the input.
        /// </summary>
        public string ColumnCssClass { get; set; }

        /// <summary>
        /// Gets or sets a suggestion for the HTML element to generate. Supported
        /// elements are all standard HTML data collection elements such as input,
        /// select, textarea, etc. Other elements are not supported.
        /// </summary>
        public string UIHint { get; set; }

        /// <summary>
        /// Gets or sets the tag or element name of the control to display.
        /// This is an alias for <see cref="UIHint"/>.
        /// </summary>
        public string Tag { get => UIHint; set => UIHint = value; }

        /// <summary>
        /// Gets or sets a suggestion for the input type to generate. Supported types are
        /// all standard HTML input types (e. g. text, checkbox, number, date, file...).
        /// All type names must be in lower-case characters.
        /// </summary>
        public string UITypeHint { get; set; }

        /// <summary>
        /// Gets or sets the type of the control to display.
        /// This is an alias for <see cref="UITypeHint"/>.
        /// </summary>
        public string Type { get => UITypeHint; set => UITypeHint = value; }

        /// <summary>
        /// Gets or sets an icon associated with the generated input.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the data format. If you want to format a <see cref="DateTime"/>
        /// or <see cref="DateTimeOffset"/>, you must specify a valid value for 
        /// <see cref="CultureName"/> and set <see cref="UITypeHint"/> to 'text'.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// The case-insensitive name of a culture to use for conversions and formatting.
        /// </summary>
        public string CultureName { get; set; }

        /// <summary>
        /// Determines the styles permitted in numeric string arguments that are passed to
        /// the Parse and TryParse methods of the integral and floating-point numeric types.
        /// </summary>
        public System.Globalization.NumberStyles NumberStyles { get; set; }

        /// <summary>
        /// Gets or sets a vertical pipe-separated list of key/value pairs of options to render for 'select' tag. 
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// Gets or sets a vertical pipe-separated list of key/value pairs of additional attributes to display.
        /// </summary>
        public string ExtraAttributes { get; set; }

        /// <summary>
        /// Indicates whether rendering custom inputs is disabled, enabled or determined by the default value.
        /// </summary>
        public CustomRenderMode CustomRenderMode { get; set; }

        /// <summary>
        /// Indicates whether the <see cref="UITypeHint"/> property value is checkbox.
        /// </summary>
        public bool IsInputCheckbox => UITypeHint.EqualNoCase("checkbox");

        /// <summary>
        /// Indicates whether the <see cref="UITypeHint"/> property value is radio.
        /// </summary>
        public bool IsInputRadio => UITypeHint.EqualNoCase("radio");

        /// <summary>
        /// Returns true if either of <see cref="IsInputCheckbox"/> or <see cref="IsInputRadio"/> is true.
        /// </summary>
        public bool IsInputCheckboxOrRadio => IsInputCheckbox || IsInputRadio;

        /// <summary>
        /// Determines whether the specified <paramref name="type"/> corresponds
        /// to the <see cref="UITypeHint"/> property value.
        /// </summary>
        /// <param name="type">The type of the HTML element to compare to.</param>
        /// <returns></returns>
        public bool Is(string type) => UITypeHint.EqualNoCase(type);

        #region Intended for infrastructure support only

        /// <summary>
        /// Gets or sets the <see cref="InputFileAttribute"/> associated with the 
        /// property that this <see cref="FormDisplayAttribute"/> decorates.
        /// </summary>
        protected internal InputFileAttribute FileAttribute { get; set; }

        /// <summary>
        /// Gets the <see cref="FileAttribute"/> property value.
        /// </summary>
        /// <returns></returns>
        public InputFileAttribute GetFileAttribute() => FileAttribute;

        /// <summary>
        /// Sets the <see cref="FileAttribute"/> property value.
        /// </summary>
        /// <param name="value"></param>
        public void SetFileAttribute(InputFileAttribute value) => FileAttribute = value;

        internal DragDropAttribute DragDropAttribute { get; set; }

        private PropertyInfo _property;

        // The following methods represent the getter and setter of the
        // otherwise public property named 'Property' of the current attribute,
        // unnecessarily exposing it to the outer world.

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> of the model's property that the current attribute decorates.
        /// </summary>
        /// <returns></returns>
        public PropertyInfo GetProperty() => _property;

        /// <summary>
        /// Sets the <see cref="PropertyInfo"/> of the model's property that the current attribute decorates.
        /// </summary>
        /// <param name="value"></param>
        public void SetProperty(PropertyInfo value) => _property = value;

        /// <summary>
        /// Returns an initialized instance of the <see cref="ResourceManager"/> 
        /// class for the <see cref="ResourceType"/> property.
        /// </summary>
        /// <returns></returns>
        public ResourceManager GetResourceManager() => ResourceType == null ? null : new ResourceManager(ResourceType);

        #endregion
    }
}
