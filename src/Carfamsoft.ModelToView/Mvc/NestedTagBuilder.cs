using Carfamsoft.ModelToView.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Carfamsoft.ModelToView.Mvc
{
    /// <summary>
    /// Contains classes and properties that are used to create nested HTML elements.
    /// </summary>
    public class NestedTagBuilder : TagBuilder
    {
        private readonly IList<NestedTagBuilder> _innerTags = new List<NestedTagBuilder>();

        /// <inheritdoc />
        public NestedTagBuilder(string tagName) : base(tagName)
        {
        }

        /// <summary>
        /// Creates a new <see cref="NestedTagBuilder"/> class that has the specified tag name.
        /// </summary>
        /// <param name="tagName">
        /// The tag name without the "&lt;", "/", or "&gt;" delimiters.
        /// </param>
        /// <returns></returns>
        public static NestedTagBuilder Create(string tagName)
        {
            return new NestedTagBuilder(tagName);
        }

        /// <summary>
        /// Gets a new read-only collection of <see cref="NestedTagBuilder"/> elements.
        /// </summary>
        public IEnumerable<NestedTagBuilder> InnerTags
        {
            get
            {
                return new ReadOnlyCollection<NestedTagBuilder>(_innerTags);
            }
        }

        /// <summary>
        /// Adds an initialized <see cref="NestedTagBuilder"/> to the underlying collection.
        /// </summary>
        /// <param name="tag">An initialized <see cref="NestedTagBuilder"/> to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tag"/> is null.</exception>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddChild(NestedTagBuilder tag)
        {
            _innerTags.Add(tag ?? throw new ArgumentNullException(nameof(tag)));
            return this;
        }

        /// <summary>
        /// Adds an initialized <see cref="NestedTagBuilder"/> to the underlying collection
        /// if the <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="callback">
        /// A function that creates a <see cref="NestedTagBuilder"/> if the <paramref name="condition"/> is true.
        /// </param>
        /// <returns></returns>
        public NestedTagBuilder AddChildIf(bool condition, Func<NestedTagBuilder> callback)
        {
            if (condition) AddChild(callback());
            return this;
        }

        /// <summary>
        /// Adds an initialized <see cref="NestedTagBuilder"/> to the underlying collection
        /// if the <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="child">The child tag to add.</param>
        /// <returns></returns>
        public NestedTagBuilder AddChildIf(bool condition, NestedTagBuilder child)
        {
            if (condition) AddChild(child);
            return this;
        }

        /// <summary>
        /// Renders the HTML tag using a self closing or normal mode.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_innerTags.Count > 0)
            {
                InnerHtml = RenderSubTags(this);
            }
            var mode = HtmlTagInfo.IsSelfClosing(TagName) ? TagRenderMode.SelfClosing : TagRenderMode.Normal;
            return ToString(mode);
        }

        private string RenderSubTags(NestedTagBuilder tag)
        {
            var sb = new StringBuilder();
            foreach (var t in tag._innerTags)
            {
                sb.Append(t.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Adds multiple attributes.
        /// </summary>
        /// <param name="additionalAttributes">A collection of key-value pairs representing attributes.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddMultipleAttributes(IEnumerable<KeyValuePair<string, object>> additionalAttributes)
        {
            if (additionalAttributes != null && additionalAttributes.Count() > 0)
            {
                var culture = CultureInfo.CurrentCulture;
                foreach (var kvp in additionalAttributes)
                {
                    if (BindConverter.TryConvertTo<string>(kvp.Value, culture, out var value))
                    {
                        MergeAttribute(kvp.Key, value);
                    }
                    else
                    {
                        MergeAttribute(kvp.Key, $"{kvp.Value}");
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// Adds a CSS class to the list of CSS classes in the tag.
        /// </summary>
        /// <param name="name">The name of the CSS class to add.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddClass(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
                AddCssClass(name);
            return this;
        }

        /// <summary>
        /// Adds a CSS class to the list of CSS classes in the tag if <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="name">The name of the CSS class to add.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddClassIf(bool condition, string name)
        {
            if (condition && !string.IsNullOrWhiteSpace(name))
            {
                AddCssClass(name);
            }
            return this;
        }

        /// <summary>
        /// Invokes the specified <paramref name="callback"/> for each item contained in the given <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements contained in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">A collection of elements for which to invoke the <paramref name="callback"/>.</param>
        /// <param name="callback">
        /// A function that creates a <see cref="NestedTagBuilder"/> for each item contained in the 
        /// <paramref name="collection"/>. If the return value for an item is null, no child tag will be added.
        /// </param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddChildForEach<T>(IEnumerable<T> collection, Func<T, NestedTagBuilder> callback)
        {
            foreach (var item in collection)
            {
                var child = callback(item);
                if (child != null) AddChild(child);
            }
            return this;
        }

        /// <summary>
        /// Invokes the specified <paramref name="callback"/> for each item contained 
        /// in the given <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements contained in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">A collection of elements for which to invoke the <paramref name="callback"/>.</param>
        /// <param name="callback">
        /// A function that is invoked for each item contained in the <paramref name="collection"/>.
        /// </param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder ForEach<T>(IEnumerable<T> collection, Action<T> callback)
        {
            foreach (var item in collection)
            {
                callback(item);
            }
            return this;
        }

        /// <summary>
        /// Invokes the specified <paramref name="callback"/> for each item contained 
        /// in the given <paramref name="collection"/> and additionally passes a reference
        /// to the current <see cref="NestedTagBuilder"/> instance in the callback arguments.
        /// </summary>
        /// <typeparam name="T">The type of the elements contained in <paramref name="collection"/>.</typeparam>
        /// <param name="collection">A collection of elements for which to invoke the <paramref name="callback"/>.</param>
        /// <param name="callback">
        /// A function that is invoked for each item contained in the <paramref name="collection"/>.
        /// The second argument contains a reference to the current <see cref="NestedTagBuilder"/>.
        /// </param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder ForEach<T>(IEnumerable<T> collection, Action<T, NestedTagBuilder> callback)
        {
            foreach (var item in collection)
            {
                callback(item, this);
            }
            return this;
        }

        /// <summary>
        ///  Adds a new attribute to the tag.
        /// </summary>
        /// <param name="key">The key for the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddAttribute(string key, string value)
        {
            MergeAttribute(key, value);
            return this;
        }

        /// <summary>
        /// Adds an attribute to the tag if <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="keyAndValue">The key and value for the attribute.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddAttributeIf(bool condition, string keyAndValue)
        {
            if (condition)
            {
                MergeAttribute(keyAndValue, keyAndValue);
            }
            return this;
        }

        /// <summary>
        /// Adds an attribute to the tag if <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="key">The key for the attribute.</param>
        /// <param name="value">The value for the attribute.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddAttributeIf(bool condition, string key, string value)
        {
            if (condition)
            {
                MergeAttribute(key, value);
            }
            return this;
        }

        /// <summary>
        /// Adds an attribute to the tag if <paramref name="keyAndValue"/> is not empty.
        /// </summary>
        /// <param name="keyAndValue">The key and value for the attribute.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddAttributeIfNotBlank(string keyAndValue)
        {
            if (keyAndValue.IsNotWhiteSpace())
                MergeAttribute(keyAndValue, keyAndValue);
            return this;
        }

        /// <summary>
        /// Adds an attribute to the tag if <paramref name="value"/> is not empty.
        /// </summary>
        /// <param name="key">The key for the attribute.</param>
        /// <param name="value">The value for the attribute.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddAttributeIfNotBlank(string key, string value)
        {
            if (value.IsNotWhiteSpace())
                MergeAttribute(key, value);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="TagBuilder.InnerHtml"/> property of the 
        /// element to an HTML-encoded version of the specified string.
        /// </summary>
        /// <param name="text">The string to HTML-encode.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder SetText(string text)
        {
            SetInnerText(text);
            return this;
        }

        /// <summary>
        /// Sets the <see cref="TagBuilder.InnerHtml"/> property of the 
        /// element to a non HTML-encoded version of the specified string.
        /// </summary>
        /// <param name="html">The inner HTML value for the element.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder SetInnerHtml(string html)
        {
            InnerHtml = html;
            return this;
        }

        /// <summary>
        /// Appends the specified content to the <see cref="TagBuilder.InnerHtml"/> property.
        /// </summary>
        /// <param name="content">The content to append.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddContent(string content)
        {
            InnerHtml += content;
            return this;
        }

        /// <summary>
        /// Appends the specified content to the <see cref="TagBuilder.InnerHtml"/> 
        /// property only if <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="condition">The condition to test.</param>
        /// <param name="createContentCallback">A function that creates the content to append.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddContentIf(bool condition, Func<string> createContentCallback)
        {
            return condition ? AddContent(createContentCallback()) : this;
        }

        /// <summary>
        /// Appends the specified content to the <see cref="TagBuilder.InnerHtml"/> 
        /// property only if <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="condition">The condition to test.</param>
        /// <param name="content">The content to append if the <paramref name="condition"/> is true.</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddContentIf(bool condition, string content)
        {
            return condition ? AddContent(content) : this;
        }

        /// <summary>
        /// Appends the specified content to the <see cref="TagBuilder.InnerHtml"/> 
        /// property only if <paramref name="content"/> is not blank.
        /// </summary>
        /// <param name="content">The content to append</param>
        /// <returns>A reference to this <see cref="NestedTagBuilder"/>.</returns>
        public NestedTagBuilder AddContentIfNotBlank(string content)
        {
            return content.IsNotWhiteSpace() ? AddContent(content) : this;
        }
    }
}