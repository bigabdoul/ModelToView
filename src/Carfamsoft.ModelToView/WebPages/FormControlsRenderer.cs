using Carfamsoft.ModelToView.Extensions;
using Carfamsoft.ModelToView.Shared;
using Carfamsoft.ModelToView.Shared.Extensions;
using Carfamsoft.ModelToView.ViewAnnotations;
using System;
using System.Text;
using System.Web.Mvc;
using System.Web.WebPages.Html;

namespace Carfamsoft.ModelToView.WebPages
{
    /// <summary>
    /// Represents an object that renders an instance of the <see cref="ControlInfo"/> 
    /// class as collection of form controls.
    /// </summary>
    [Obsolete("Use Carfamsoft.ModelToView.WebPages.AutoInputBase")]
    public class FormControlsRenderer : IControlRenderer
    {
        #region fields

        private ControlInfo _info;

        private static readonly string[] FormControlTypes = new string[]
        {
            "date",
            "datetime-local",
            "email",
            "file",
            "number",
            "password",
            "search",
            "tel",
            "text",
            "time",
            "url",
            "week",
            "select",
            "textarea",
            /*
            <input type="button">
            <input type="checkbox">
            <input type="color">
            <input type="date">
            <input type="datetime-local">
            <input type="email">
            <input type="file">
            <input type="hidden">
            <input type="image">
            <input type="month">
            <input type="number">
            <input type="password">
            <input type="radio">
            <input type="range">
            <input type="reset">
            <input type="search">
            <input type="submit">
            <input type="tel">
            <input type="text">
            <input type="time">
            <input type="url">
            <input type="week">
             */
        };

        #endregion

        /// <summary>
        /// Initializes a new isntance of the <see cref="FormControlsRenderer"/> class.
        /// </summary>
        /// <param name="renderOptions">The options for rendering the object as a collection of HTML controls.</param>
        public FormControlsRenderer(ControlRenderOptions renderOptions = null)
        {
            RenderOptions = renderOptions;
        }

        /// <summary>
        /// Gets or sets the options for rendering an HTML control.
        /// </summary>
        public ControlRenderOptions RenderOptions { get; set; }

        /// <summary>
        /// Renders the underlying <see cref="ControlInfo"/>.
        /// </summary>
        /// <returns>An HTML string that represents the rendered <see cref="ControlInfo"/> instance.</returns>
        public virtual string Render(ControlInfo info)
        {
            _info = info ?? throw new ArgumentNullException(nameof(info));
            
            if (_info.Tag.EqualNoCase("textarea"))
            {
                return RenderTextArea();
            }

            /* This is a sample of what is rendered here:
            <div class="form-group">
                <div class="input-group">
                    <label class="input-group-addon" for="FirstName_a894946b">
                        <span class="input-group-text"><i class="fas fa-user"></i> First Name</span>
                    </label>
                    <input type="text" id="FirstName_a894946b" name="FirstName" ng-model="model.firstName" class="form-control" required />
                </div>
            </div>
            */

            var formgroup = new TagBuilder("div");
            var inputGroup = new TagBuilder("div");
            var label = new TagBuilder("label");
            var span = new TagBuilder("span");
            var control = new TagBuilder(_info.Tag);

            span.InnerHtml = RenderIcon();
            RenderDisplay(span);

            formgroup.AddCssClass("form-group");
            inputGroup.AddCssClass("input-group");
            span.AddCssClass("input-group-text");
            label.AddCssClass("input-group-addon");

            var value = $"{_info.Value}";

            if (control.TagName.EqualNoCase("select"))
            {
                control.InnerHtml = RenderSelectOptions();
            }
            else if (value.IsNotWhiteSpace())
            {
                control.Attributes.Add("value", value);
            }

            var controlId = RenderAttributes(control);

            if (controlId.IsNotWhiteSpace())
                label.Attributes.Add("for", controlId);

            label.InnerHtml = span.ToString();

            inputGroup.InnerHtml = label.ToString() + (HtmlTagInfo.IsSelfClosing(control.TagName)
                ? control.ToString(TagRenderMode.SelfClosing)
                : control.ToString());

            formgroup.InnerHtml = inputGroup.ToString();
            return formgroup.ToString();
        }

        /// <summary>
        /// Renders a textarea element.
        /// </summary>
        /// <returns></returns>
        protected virtual string RenderTextArea()
        {
            /* This is a sample of what is rendered here:
             <div class="form-group">
                <label for="Message_a894946b"><i class="fa fa-envelope"></i> Message (optionnel)</label>
                <textarea id="Message_a894946b" name="Message" class="form-control" rows="3" ng-model="model.message"></textarea>
            </div>
             */
            var formgroup = new TagBuilder("div");
            var label = new TagBuilder("label");
            var textarea = new TagBuilder(_info.Tag);
            var controlId = RenderAttributes(textarea);

            formgroup.AddCssClass("form-group");

            if (controlId.IsNotWhiteSpace())
                label.Attributes.Add("for", controlId);

            label.InnerHtml = RenderIcon();

            RenderDisplay(label);

            textarea.Attributes.Remove("type");
            textarea.InnerHtml = $"{_info.Value}";
            formgroup.InnerHtml = label.ToString() + textarea.ToString();

            return formgroup.ToString();
        }

        /// <summary>
        /// Renders the icon as an HTML string.
        /// </summary>
        /// <returns></returns>
        protected virtual string RenderIcon()
        {
            var strIcon = _info.DisplayHint?.Icon;

            if (strIcon.IsNotWhiteSpace())
            {
                var iBuilder = new TagBuilder("i");
                iBuilder.AddCssClass(strIcon);
                return iBuilder.ToString() + "&nbsp;";
            }

            return string.Empty;
        }

        /// <summary>
        /// Renders the display name and prompt to the specified <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The tag builder to render to.</param>
        protected virtual void RenderDisplay(TagBuilder builder)
        {
            var display = _info.Display;
            if (display != null)
            {
                builder.InnerHtml += _info.GetDisplayString(display.Name ?? _info.Name);

                if (display.Prompt.IsNotWhiteSpace())
                    builder.Attributes.Add("title", _info.GetDisplayString(display.Prompt));
            }
            else
            {
                builder.InnerHtml += _info.GetDisplayString(_info.Name);
            }
        }

        /// <summary>
        /// Renders a collection of 'option' elements as an HTML string.
        /// </summary>
        /// <returns></returns>
        protected virtual string RenderSelectOptions()
        {
            var sb = new StringBuilder();

            void AddOptions(string values)
            {
                var selectedValue = $"{_info.Value}";

                foreach (var kvp in DisplayHintAttribute.ParseKeyValuePairs(values))
                {
                    var option = new TagBuilder("option");

                    option.Attributes.Add("value", kvp.Key);

                    if (selectedValue == kvp.Key)
                        option.Attributes.Add("selected", "selected");

                    option.InnerHtml = _info.GetDisplayString(kvp.Value);

                    sb.Append(option.ToString());
                }
            }

            if (true == _info.DisplayHint?.Options.IsNotWhiteSpace())
            {
                AddOptions(_info.DisplayHint.Options);
            }
            else if (_info.Value is System.Collections.IEnumerable enumerable)
            {
                if (enumerable is string strOptions)
                {
                    AddOptions(strOptions);
                }
                else
                {
                    var selectedValue = $"{_info.Value}";

                    foreach (var e in enumerable)
                    {
                        var option = new TagBuilder("option");
                        string value, text;

                        if (e is SelectListItem item)
                        {
                            value = item.Value; 
                            text = item.Text;
                        }
                        else
                        {
                            value = text = $"{e}";
                        }

                        if (selectedValue == value)
                            option.Attributes.Add("selected", "selected");

                        option.Attributes.Add("value", value);
                        option.InnerHtml = _info.GetDisplayString(text);

                        sb.Append(option.ToString());
                    }
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Renders all attributes to the <paramref name="builder"/>'s attributes collection.
        /// </summary>
        /// <param name="builder">The tag builder to manipulate.</param>
        /// <returns>A string that represents the unique identifier of the rendered control.</returns>
        protected virtual string RenderAttributes(TagBuilder builder)
        {
            _info.DisplayHint?.GetExtraAttributes().MergeAttributes(_info.Attributes);

            var options = RenderOptions;
            var controlId = (options?.GenerateIdAttribute == true) ? GenerateControlId() : string.Empty;
            var attrs = builder.Attributes;
            var shouldAddTypeAttr = HtmlTagInfo.HasTypeAttribute(_info.Tag);

            if (true == options?.GenerateNameAttribute)
                attrs.Add("name", _info.Name);

            if (controlId.IsNotWhiteSpace())
                attrs.Add("id", controlId);

            // 'text' is the default value for the type attribute
            if (shouldAddTypeAttr && _info.Type.IsNotWhiteSpace() && !_info.Type.EqualNoCase("text"))
                attrs.Add("type", _info.Type);

            var ngModel = options?.NgModel;

            if (ngModel != null)
            {
                if (ngModel.Length == 0)
                    attrs["ng-model"] = _info.Name.ToCamelCase();
                else
                    attrs["ng-model"] = $"{ngModel}.{_info.Name.ToCamelCase()}";
            }

            var ovalue = _info.Range?.Minimum;

            if (ovalue != null)
                attrs["min"] = $"{ovalue}";

            ovalue = _info.Range?.Maximum;

            if (ovalue != null)
                attrs["max"] = $"{ovalue}";

            var ivalue = _info.StringLength?.MaximumLength;

            if (ivalue.HasValue && ivalue.Value >= 0)
                attrs["maxlen"] = $"{ivalue.Value}";

            if (_info.IsRequired)
                attrs["required"] = "required";

            var defaultCss = options?.DefaultCssClass;

            if (defaultCss == null)
                defaultCss = "form-control";

            if (defaultCss.IsNotWhiteSpace() &&
                (FormControlTypes.ContainsNoCase(builder.TagName) ||
                builder.TagName.EqualNoCase("input") &&
                (_info.Type.IsWhiteSpace() || FormControlTypes.ContainsNoCase(_info.Type))))
            {
                attrs["class"] = defaultCss;
            }

            if (!shouldAddTypeAttr) attrs.Remove("type");

            return controlId;
        }

        private string GenerateControlId() => $"{_info.Name}_{Guid.NewGuid().GetHashCode():x}";
    }
}
