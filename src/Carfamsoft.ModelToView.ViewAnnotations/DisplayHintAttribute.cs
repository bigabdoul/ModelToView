using Carfamsoft.ModelToView.Shared;
using Carfamsoft.ModelToView.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Carfamsoft.ModelToView.ViewAnnotations
{
    /// <summary>
    /// Specifies the control used to display a data field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [Obsolete("Use Carfamsoft.ModelToView.ViewAnnotations.FormDisplayAttribute")]
    public class DisplayHintAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the tag or element name of the control to display.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the type of the control to display.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets a vertical pipe-separated list of key/value pairs of additional attributes to display.
        /// </summary>
        public string ExtraAttributes { get; set; }

        /// <summary>
        /// Gets or sets a vertical pipe-separated list of key/value pairs of options to render for 'select' tag. 
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// Gets or sets the icon for the control's label.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Parses the <see cref="ExtraAttributes"/> property's value into a case-insensitive string dictionary.
        /// </summary>
        /// <returns></returns>
        public virtual IDictionary<string, string> GetExtraAttributes()
        {
            return ParseKeyValuePairs(ExtraAttributes);
        }

        /// <summary>
        /// Parses the <paramref name="values"/> into a case-insensitive string dictionary.
        /// </summary>
        /// <param name="values">A vertical pipe-separated list of key/value pairs.</param>
        /// <returns></returns>
        public static IDictionary<string, string> ParseKeyValuePairs(string values)
        {
            return values.ParseKeyValuePairs();
        }
    }
}
