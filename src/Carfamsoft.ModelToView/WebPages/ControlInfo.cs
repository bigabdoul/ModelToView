using Carfamsoft.ModelToView.Shared;
using Carfamsoft.ModelToView.Shared.Extensions;
using Carfamsoft.ModelToView.ViewAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Resources;

namespace Carfamsoft.ModelToView.WebPages
{
    /// <summary>
    /// Encapsulates metadata about an HTML control.
    /// </summary>
    [Obsolete]
    public class ControlInfo : IComparable<ControlInfo>
    {
        #region fields

        private ResourceManager _resourceManager;
        private DisplayAttribute _display;
        private IDictionary<string, string> _attributes;
        private Type _nullableUnderlyingType;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlInfo"/> class.
        /// </summary>
        public ControlInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlInfo"/> class using the specified parameters.
        /// </summary>
        /// <param name="tag">The HTML tag name.</param>
        /// <param name="name">The name of the control.</param>
        /// <param name="propertyType">The property type for which this <see cref="ControlInfo"/> is being initialized.</param>
        public ControlInfo(string tag, string name, Type propertyType)
        {
            Tag = tag;
            Name = name;

            if (propertyType != null)
            {
                PropertyType = propertyType;
                _nullableUnderlyingType = Nullable.GetUnderlyingType(propertyType);
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets the HTML tag name.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the type of HTML control.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the control.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the value of the HTML control to render.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Indicates whether the control's value is required.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the string length attribute.
        /// </summary>
        public StringLengthAttribute StringLength { get; set; }

        /// <summary>
        /// Gets or sets the range attribute;
        /// </summary>
        public RangeAttribute Range { get; set; }

        /// <summary>
        /// Gets or sets the display attribute.
        /// </summary>
        public DisplayAttribute Display
        {
            get => _display;
            set
            {
                _display = value;
                if (_display?.ResourceType != null)
                {
                    _resourceManager = new ResourceManager(_display.ResourceType);
                }
            }
        }

        /// <summary>
        /// Gets or sets the display hint attribute.
        /// </summary>
        public DisplayHintAttribute DisplayHint { get; set; }

        /// <summary>
        /// Gets or sets the resource manager to use.
        /// </summary>
        public ResourceManager ResourceManager { get => _resourceManager; set => _resourceManager = value; }

        /// <summary>
        /// Gets a collection of custom control attributes.
        /// </summary>
        public IDictionary<string, string> Attributes => _attributes ?? (_attributes = new Dictionary<string, string>());

        /// <summary>
        /// Gets the type of the property for which this <see cref="ControlInfo"/> was generated.
        /// </summary>
        public Type PropertyType { get; }

        /// <summary>
        /// Returns a localized string for a property of the <see cref="Display"/> attribute.
        /// </summary>
        /// <param name="name">The name of the resource to retrieve.</param>
        /// <param name="culture">An object that represents the culture for which the resource is localized.</param>
        /// <returns>The value of the resource localized for the specified culture, or null if name cannot be found in a resource set.</returns>
        public string GetDisplayString(string name, System.Globalization.CultureInfo culture = null)
            => _resourceManager.GetDisplayString(name, culture);

        int IComparable<ControlInfo>.CompareTo(ControlInfo other) => CompareTo(other);

        private int CompareTo(ControlInfo other)
        {
            if (other == null) return 1;

            // equality if both Displays are null
            if (Display == null && other.Display == null)
                return 0;

            var order1 = Display?.GetOrder() ?? int.MaxValue;
            var order2 = other.Display?.GetOrder() ?? int.MaxValue;

            // compare their Orders
            return order1.CompareTo(order2);
        }
    }
}
