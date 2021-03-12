using System;
using System.Collections.Generic;

namespace Carfamsoft.ModelToView.Shared
{
    /// <summary>
    /// Represents an object that controls a part of an HTML generation process.
    /// </summary>
    public class ControlRenderOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlRenderOptions"/> class.
        /// </summary>
        public ControlRenderOptions()
        {
        }

        /// <summary>
        /// Returns a new instance if the <see cref="ControlRenderOptions"/> class using default values.
        /// </summary>
        /// <returns></returns>
        public static ControlRenderOptions CreateDefault() => new ControlRenderOptions
        {
            RowCssClass = "row",
            DefaultCssClass = "form-control",
            GenerateIdAttribute = true,
            GenerateNameAttribute = true,
            CamelCaseId = true
        };

        /// <summary>
        /// Gets the default control render options.
        /// </summary>
        public static readonly ControlRenderOptions Default = CreateDefault();

        /// <summary>
        /// Gets or sets the CSS class name for a grouped row.
        /// </summary>
        public string RowCssClass { get; set; }

        /// <summary>
        /// Gets or sets the CSS class name for a column in a grouped row.
        /// </summary>
        public string ColCssClass { get; set; }

        /// <summary>
        /// Gets or sets the HTML tag name for the element that wraps around a group of properties.
        /// </summary>
        public string GroupWrapperTagName { get; set; }

        /// <summary>
        /// Gets or sets the HTML tag name for the element that acts a the title for the group.
        /// </summary>
        public string GroupHeaderTagName { get; set; }

        /// <summary>
        /// Gets or sets the CSS class name for a group.
        /// </summary>
        public string GroupNameCssClass { get; set; }

        /// <summary>
        /// Gets or sets the CSS class name for a property.
        /// </summary>
        public string PropertyNameCssClass { get; set; }

        /// <summary>
        /// Indicates whether the 'id' attribute for a control should be computed.
        /// </summary>
        public bool GenerateIdAttribute { get; set; }

        /// <summary>
        /// Indicates whether the 'name' attribute for a control should be added.
        /// </summary>
        public bool GenerateNameAttribute { get; set; }

        /// <summary>
        /// Gets or sets the model prefix for the AngularJS ng-model attribute.
        /// If the value is null the ng-mnodel attribute is not added.
        /// If the value is empty the attribute is added without the prefix.
        /// </summary>
        public string NgModel { get; set; }

        /// <summary>
        /// Gets or sets the default CSS class for a rendered control.
        /// </summary>
        public string DefaultCssClass { get; set; }

        /// <summary>
        /// Gets or sets a function callback used to retrieve a collection <see cref="SelectOption"/>
        /// elements to use for a 'select' element or an input or type 'radio'.
        /// </summary>
        public Func<string, IEnumerable<SelectOption>> OptionsGetter { get; set; }

        /// <summary>
        /// Gets or sets a function callback used to dynamically retrieve the disabled state of an input.
        /// </summary>
        public Func<string, bool?> DisabledGetter { get; set; }
        
        /// <summary>
        /// Gets or sets a function callback used to retrieve additional attributes.
        /// </summary>
        public Func<string, IReadOnlyDictionary<string, object>> AdditionalAttributesGetter { get; set; }

        /// <summary>
        /// Gets or sets the unique control identifier.
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// Determines whether to use camel-casing when generating the 'id' attribute.
        /// </summary>
        public bool CamelCaseId { get; set; }
    }
}
