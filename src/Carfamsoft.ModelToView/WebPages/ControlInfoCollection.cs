using Carfamsoft.ModelToView.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Resources;

namespace Carfamsoft.ModelToView.WebPages
{
    /// <summary>
    /// Represents a collection of <see cref="ControlInfo"/> elements which encapsulates extra properties.
    /// </summary>
    [Obsolete]
    public class ControlInfoCollection : IReadOnlyCollection<ControlInfo>
    {
        private readonly ICollection<ControlInfo> _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlInfoCollection"/> class using the specified collection.
        /// </summary>
        /// <param name="collection">An initialized collection of <see cref="ControlInfo"/> elements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public ControlInfoCollection(ICollection<ControlInfo> collection)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        /// <summary>
        /// Gets or sets the <see cref="ResourceManager"/> for this control info list.
        /// </summary>
        public ResourceManager ResourceManager { get; set; }

        /// <summary>
        /// Safely returns the value of the string resource localized for the specified culture.
        /// </summary>
        /// <param name="name">The name of the resource to retrieve.</param>
        /// <param name="culture">An object that represents the culture for which the resource is localized.</param>
        /// <returns>The value of the resource localized for the specified culture, or null if name cannot be found in a resource set.</returns>
        public string GetString(string name, System.Globalization.CultureInfo culture = null)
        {
            if (name.IsNotWhiteSpace() && ResourceManager != null)
            {
                try
                {
                    var result = ResourceManager.GetString(name, culture);
                    if (result.IsWhiteSpace()) result = name.TitleCaseWords();
                    return result;
                }
                catch
                {
                }
            }
            return name.TitleCaseWords();
        }

        #region IReadOnlyCollection<ControlInfo>

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        /// Gets the number of elements contained in the list.
        /// </summary>
        public int Count => _collection.Count;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ControlInfo> GetEnumerator() => _collection.GetEnumerator();

        #endregion
    }
}
