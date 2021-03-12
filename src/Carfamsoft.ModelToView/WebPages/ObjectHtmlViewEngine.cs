using Carfamsoft.ModelToView.Shared;
using Carfamsoft.ModelToView.ViewAnnotations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;

namespace Carfamsoft.ModelToView.WebPages
{
    /// <summary>
    /// Provides methods to generate HTML output from an object.
    /// </summary>
    [Obsolete]
    public class ObjectHtmlViewEngine
    {
        #region fields

        private readonly object _model;

        #region static

        private static readonly ConcurrentDictionary<Type, ControlInfoCollection> ControlListCache
            = new ConcurrentDictionary<Type, ControlInfoCollection>();

        private const BindingFlags READ_WRITE_PROPERTIES = BindingFlags.Public |
            BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty;

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectHtmlViewEngine"/> class using the specified model.
        /// </summary>
        /// <param name="model">The object used to generate HTML output.</param>
        public ObjectHtmlViewEngine(object model) : this(model, (ControlRenderOptions)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectHtmlViewEngine"/> class using the specified parameters.
        /// </summary>
        /// <param name="model">The object used to generate HTML output.</param>
        /// <param name="options">The options for rendering the object as a collection of HTML controls.</param>
        public ObjectHtmlViewEngine(object model, ControlRenderOptions options)
            : this(model, new FormControlsRenderer { RenderOptions = options ?? ControlRenderOptions.Default })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectHtmlViewEngine"/> class using the specified parameters.
        /// </summary>
        /// <param name="model">The object used to generate HTML output.</param>
        /// <param name="renderer">The <see cref="IControlRenderer"/> used to render instances of the <see cref="ControlInfo"/> class.</param>
        /// <exception cref="ArgumentNullException"><paramref name="model"/> is null.</exception>
        public ObjectHtmlViewEngine(object model, IControlRenderer renderer)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            Renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }

        /// <summary>
        /// Gets the <see cref="IControlRenderer"/> used to render instances of the <see cref="ControlInfo"/> class.
        /// </summary>
        public IControlRenderer Renderer { get; }

        /// <summary>
        /// Renders the underlying object as an HTML string.
        /// </summary>
        /// <returns></returns>
        public virtual string Render()
        {
            if (_model == null) return null;

            var infoList = GetControlInfoList(_model);

            if (infoList.Count > 0)
            {
                var builder = new StringBuilder();

                RenderGroups(infoList, builder);
                RenderNonGroups(infoList, builder);

                return builder.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Renders the specified collection of <see cref="ControlInfo"/> 
        /// elements by grouping the underlying object whose properties have 
        /// specified the <see cref="DisplayAttribute.GroupName"/> property.
        /// </summary>
        /// <param name="collection">The collection to group and render.</param>
        /// <param name="builder">The string builder that collects the output.</param>
        protected virtual void RenderGroups(ControlInfoCollection collection, StringBuilder builder)
        {
            var tempInfos = collection
                .Where(i => i.Display != null && i.Display.GroupName.IsNotWhiteSpace())
                .GroupBy(i => i.Display.GroupName)
                .ToArray();

            if (tempInfos.Length > 0)
            {
                var cfg = Renderer?.RenderOptions;
                var rowClass = cfg?.RowCssClass ?? "row";
                var colClass = cfg?.ColCssClass;
                var groupWrapperTagName = cfg?.GroupWrapperTagName;
                var groupHeaderTagName = cfg?.GroupHeaderTagName;
                var groupNameCssClass = cfg?.GroupNameCssClass;
                var propertyNameCssClass = cfg?.PropertyNameCssClass;

                var addColClassSuffix = false;
                var hasRowClass = rowClass.IsNotWhiteSpace();
                var includeGroupName = groupHeaderTagName.IsNotWhiteSpace();
                var hasGroupWrapper = groupWrapperTagName.IsNotWhiteSpace();
                var omitGroupNameCssClass = groupNameCssClass.IsWhiteSpace();
                var omitPropertyNameCssClass = propertyNameCssClass.IsWhiteSpace();

                if (colClass == null)
                {
                    colClass = "col-md-";
                    addColClassSuffix = true;
                }

                var hasColClass = colClass.IsNotWhiteSpace();

                string BuildGroupName(string groupName)
                {
                    if (includeGroupName)
                    {
                        var nameBuilder = new TagBuilder(groupHeaderTagName)
                        {
                            InnerHtml = collection.GetString(groupName)
                        };
                        return nameBuilder.ToString();
                    }
                    return string.Empty;
                }

                foreach (var g in tempInfos)
                {
                    var lstSorted = new List<ControlInfo>(g);
                    var colen = lstSorted.Count; // column length

                    if (colen == 0) continue;
                    
                    lstSorted.Sort();

                    var sbRow = new StringBuilder();
                    var rowBuilder = new TagBuilder("div");

                    if (hasRowClass) rowBuilder.AddCssClass(rowClass);

                    if (hasColClass && addColClassSuffix)
                    {
                        if (colen >= 4) colen = 4;
                        colen = 12 / colen; // 3,4,6,12 for col-md-3, col-md-4, etc.
                    }

                    foreach (var info in lstSorted)
                    {
                        var colBuilder = new TagBuilder("div")
                        {
                            // Render the info
                            InnerHtml = Render(info)
                        };

                        if (hasColClass)
                        {
                            var cls = colClass;

                            if (addColClassSuffix)
                                cls += colen;

                            if (!omitPropertyNameCssClass)
                                cls = $"{cls} {propertyNameCssClass} {info.Name.TitleCaseWords().SanitizeName()}";

                            colBuilder.AddCssClass(cls);
                        }

                        sbRow.Append(colBuilder.ToString());
                    }

                    rowBuilder.InnerHtml = sbRow.ToString();

                    if (hasGroupWrapper)
                    {
                        var wrapperBuilder = new TagBuilder(groupWrapperTagName)
                        {
                            InnerHtml = BuildGroupName(g.Key)
                        };

                        if (!omitGroupNameCssClass)
                            wrapperBuilder.AddCssClass($"{groupNameCssClass} {g.Key.TitleCaseWords().SanitizeName()}");

                        wrapperBuilder.InnerHtml += rowBuilder.ToString();

                        builder.Append(wrapperBuilder.ToString());
                    }
                    else
                    {
                        builder.Append(BuildGroupName(g.Key)).Append(rowBuilder.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Renders the specified collection of <see cref="ControlInfo"/> 
        /// elements by grouping the underlying object whose properties have
        /// not specified the <see cref="DisplayAttribute.GroupName"/> property.
        /// </summary>
        /// <param name="collection">The collection render.</param>
        /// <param name="builder">The string builder that collects the output.</param>
        protected virtual void RenderNonGroups(ControlInfoCollection collection, StringBuilder builder)
        {
            var tempInfos = collection.Where(i => i.Display == null || i.Display.GroupName.IsWhiteSpace()).ToList();

            if (tempInfos.Count > 0)
            {
                tempInfos.Sort();

                var propertyNameCssClass = Renderer?.RenderOptions?.PropertyNameCssClass;
                var omitPropertyNameCssClass = propertyNameCssClass.IsNotWhiteSpace();

                foreach (var info in tempInfos)
                {
                    var tb = new TagBuilder("div");

                    if (!omitPropertyNameCssClass)
                        tb.AddCssClass($"{propertyNameCssClass} {info.Name.TitleCaseWords().SanitizeName()}".Trim());

                    tb.InnerHtml = Render(info);
                    builder.Append(tb.ToString());
                }
            }
        }

        /// <summary>
        /// Renders the specified control info.
        /// </summary>
        /// <param name="info">The control info to render.</param>
        /// <returns></returns>
        protected virtual string Render(ControlInfo info)
        {
            return Renderer.Render(info);
        }

        /// <summary>
        /// Extracts a collection of <see cref="ControlInfo"/> metadata from the specified object.
        /// </summary>
        /// <param name="obj">The object to analyze.</param>
        /// <returns></returns>
        public static ControlInfoCollection GetControlInfoList(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            var type = obj.GetType();

            if (ControlListCache.TryGetValue(type, out var collection))
                return collection;

            var resourceManager = type.GetCustomAttribute<DisplayResourceAttribute>(true)?.GetResourceManager();
            var list = new List<ControlInfo>();

            foreach (var pi in type.GetProperties(READ_WRITE_PROPERTIES).Where(p => p.CanRead && p.CanWrite))
            {
                var info = GetControlInfo(pi);
                if (info == null) continue;

                info.Value = pi.GetValue(obj);

                if (info.ResourceManager == null)
                    info.ResourceManager = resourceManager;

                list.Add(info);
            }

            // make sure the cache is always updated
            collection = new ControlInfoCollection(list) { ResourceManager = resourceManager };
            return ControlListCache.AddOrUpdate(type, collection, (_, oldValue) => collection);
        }

        /// <summary>
        /// Extracts <see cref="ControlInfo"/> metadata from a property.
        /// </summary>
        /// <param name="pi">The property to analyze.</param>
        /// <returns></returns>
        public static ControlInfo GetControlInfo(PropertyInfo pi)
        {
            if (pi.GetCustomAttribute<DisplayIgnoreAttribute>(true) != null) return null;

            var info = new ControlInfo("input", pi.Name, pi.PropertyType)
            {
                Display = pi.GetCustomAttribute<DisplayAttribute>(true),
                DisplayHint = pi.GetCustomAttribute<DisplayHintAttribute>(true),
                Range = pi.GetCustomAttribute<RangeAttribute>(true),
                StringLength = pi.GetCustomAttribute<StringLengthAttribute>(true),
                IsRequired = pi.GetCustomAttribute<RequiredAttribute>(true) != null,
            };

            var hint = info.DisplayHint;

            if (hint != null)
            {
                if (hint.Tag.IsNotWhiteSpace())
                    info.Tag = hint.Tag;

                if (hint.Type.IsNotWhiteSpace())
                    info.Type = hint.Type;
            }

            if (info.Type.IsWhiteSpace())
            {
                var dataType = pi.GetCustomAttribute<DataTypeAttribute>(true);

                if (dataType != null)
                    info.Type = dataType.GetControlType();
                else if (pi.GetCustomAttribute<EmailAddressAttribute>(true) != null)
                    info.Type = "email";
                else
                    // check for other custom attributes that might 
                    // give us a hint about the type of control to render
                    info.Type = pi.PropertyType.GetControlType();
            }

            return info;
        }

        /// <summary>
        /// Returns a string that represents an HTML element type based on the specified <paramref name="dataType"/>.
        /// </summary>
        /// <param name="dataType">A <see cref="DataTypeAttribute"/> custom attribute to interpret.</param>
        /// <returns></returns>
        public static string GetControlType(DataTypeAttribute dataType) => dataType.GetControlType();

        /// <summary>
        /// Translates the system <paramref name="type"/> to an HTML control type.
        /// </summary>
        /// <param name="type">Ther system type to translate.</param>
        /// <returns></returns>
        public static string GetControlType(Type type) => type.GetControlType();
    }
}
