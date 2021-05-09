using Carfamsoft.ModelToView.Shared;
using Carfamsoft.ModelToView.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Resources;

namespace Carfamsoft.ModelToView.ViewAnnotations
{
    /// <summary>
    /// Holds metadata required for rendering an auto-generated HTML form element.
    /// </summary>
    public class AutoInputMetadata
    {
        private readonly string _propertyName;
        private readonly ResourceManager _resourceManager;
        private RequiredAttribute _requiredAttr;
        private bool _requiredVisited;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoInputMetadata"/> class using the specified parameters.
        /// </summary>
        /// <param name="attr">An object that encapsulates display-related metadata.</param>
        /// <param name="resourceManager">The resource manager used to retrieve localized strings.</param>
        public AutoInputMetadata(FormDisplayAttribute attr, ResourceManager resourceManager = null)
        {
            Attribute = attr ?? throw new ArgumentNullException(nameof(attr));
            PropertyInfo = attr.GetProperty() ?? throw new ArgumentNullException(nameof(PropertyInfo));
            _propertyName = PropertyInfo.Name;
            _resourceManager = resourceManager;
            ExtractOptions();
        }

        /// <summary>
        /// Gets a collection of <see cref="SelectOption"/> items for a 'select'
        /// or 'input' element of type 'radio' to generate.
        /// </summary>
        public IEnumerable<SelectOption> Options { get; private set; }

        /// <summary>
        /// Returns the display name for an input.
        /// </summary>
        /// <returns></returns>
        public string GetDisplayName() => GetDisplayString(Attribute.Name ?? _propertyName);

        /// <summary>
        /// Returns a localized string for a property of the <see cref="FormDisplayAttribute"/> attribute.
        /// </summary>
        /// <param name="name">The name of the resource to retrieve.</param>
        /// <param name="culture">
        /// An object that represents the culture for which the resource is localized.
        /// </param>
        /// <returns>
        /// The value of the resource localized for the specified culture, 
        /// or null if name cannot be found in a resource set.
        /// </returns>
        public string GetDisplayString(string name, System.Globalization.CultureInfo culture = null)
            => _resourceManager.GetDisplayString(name, culture);

        /// <summary>
        /// Indicates whether the input should be of type checkbox.
        /// </summary>
        public bool IsInputCheckbox => PropertyInfo.PropertyType.SupportsCheckbox(Attribute.UITypeHint);

        /// <summary>
        /// Returns the property value of a specified object.
        /// </summary>
        /// <param name="obj">The object whose property value will be returned.</param>
        /// <returns>The property value of the specified object.</returns>
        public object GetValue(object obj)
        {
            return PropertyInfo.GetValue(obj);
        }

        /// <summary>
        /// Gets the information about the property.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets the <see cref="FormDisplayAttribute"/>.
        /// </summary>
        public FormDisplayAttribute Attribute { get; }

        /// <summary>
        /// Gets the <see cref="RequiredAttribute"/> custom attribute associated with this metadata.
        /// </summary>
        public RequiredAttribute Required
        {
            get
            {
                if (!_requiredVisited && _requiredAttr == null)
                {
                    _requiredAttr = PropertyInfo.GetCustomAttribute<RequiredAttribute>(true);
                    _requiredVisited = true;
                }
                return _requiredAttr;
            }
        }

        /// <summary>
        /// Determines whether a value is required for the property associated with this metadata.
        /// </summary>
        public bool IsRequired => Required != null;

        #region private/internal

        /// <summary>
        /// When implemented, extracts a range of values from the custom attribute 
        /// 'RangeAttribute' using the <see cref="PropertyInfo"/> property.
        /// </summary>
        private void ExtractOptions()
        {
            if (Attribute.UIHint.EqualNoCase("select") || Attribute.UITypeHint.EqualNoCase("radio"))
            {
                var attr = PropertyInfo.GetCustomAttribute<RangeAttribute>();
                if (attr != null)
                {
                    Options = attr.OptionsFromRange(localizer: name => GetDisplayString(name));
                }
                else
                {
                    ExtractOptionsFromString();
                }
            }
        }

        private void ExtractOptionsFromString()
        {
            var values = Attribute.Options;

            if (values.IsWhiteSpace()) return;

            var list = new List<SelectOption>();

            foreach (var kvp in values.ParseKeyValuePairs())
            {
                list.Add(new SelectOption
                {
                    Id = kvp.Key,
                    Value = GetDisplayString(kvp.Value),
                });
            }

            Options = list.ToArray();
        }

        #endregion
    }
}