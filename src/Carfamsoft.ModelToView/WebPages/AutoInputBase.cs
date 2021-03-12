using Carfamsoft.ModelToView.Mvc;
using Carfamsoft.ModelToView.Shared;
using Carfamsoft.ModelToView.Shared.Extensions;
using Carfamsoft.ModelToView.ViewAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Carfamsoft.ModelToView.WebPages
{
    /// <summary>
    /// A base class for form input components generated on the fly.
    /// </summary>
    public class AutoInputBase
    {
        #region fields

        private Type _propertyType;
        private FormDisplayAttribute _metadataAttribute;
        private Type _nullableUnderlyingType;
        private string _stepAttributeValue; // Null by default, so only allows whole numbers as per HTML spec
        private CultureInfo _culture;
        private string _format;
        private string _inputId;
        private readonly ControlRenderOptions _renderOptions;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoInputBase"/> class.
        /// </summary>
        /// <param name="metadata">The metadata for generating a form input.</param>
        /// <param name="viewModel">An object used to obtain the value of the associated property defined in <paramref name="metadata"/>.</param>
        /// <param name="renderOptions">An object that controls a part of the HTML generation process.</param>
        /// <exception cref="ArgumentNullException"><paramref name="metadata"/> is null.</exception>
        public AutoInputBase(AutoInputMetadata metadata, object viewModel, ControlRenderOptions renderOptions = null)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Value = metadata.GetValue(viewModel ?? throw new ArgumentNullException(nameof(viewModel)));

            _renderOptions = renderOptions;
            _metadataAttribute = metadata.Attribute;
            CssClass = _metadataAttribute.InputCssClass;

            var propertyInfo = metadata.PropertyInfo;

            _propertyType = propertyInfo.PropertyType;
            _nullableUnderlyingType = Nullable.GetUnderlyingType(_propertyType);
            _inputId = _renderOptions?.GenerateIdAttribute ?? true ? (_renderOptions?.UniqueId ?? propertyInfo.Name.GenerateId()) : null;

            CheckIfInputNumber();
            InitCultureAndFormat();
        }

        #region properties

        /// <summary>
        /// Gets or sets metadata for generating a form input.
        /// </summary>
        public AutoInputMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the current value as a string.
        /// </summary>
        public string CurrentValueAsString { get; set; }

        /// <summary>
        /// Gets or sets a collection of additional attributes that will be applied to the created element.
        /// </summary>
        public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; } 

        #endregion

        /// <summary>
        /// Renders the component to the specified <see cref="NestedTagBuilder"/>.
        /// </summary>
        /// <param name="builder">A <see cref="NestedTagBuilder"/> that will receive the render output.</param>
        /// <returns></returns>
        public virtual NestedTagBuilder BuildRenderTree(NestedTagBuilder builder)
        {
            var elementName = GetElement(out var elementType);

            if (_metadataAttribute.IsInputRadio)
            {
                RenderRadioOptions(builder);
            }
            else if (_propertyType.SupportsCheckbox(elementType))
            {
                if (_metadataAttribute.CustomRenderMode == CustomRenderMode.Enabled)
                    RenderCustomFormCheck(builder, false, Metadata.GetDisplayName(), Metadata.PropertyInfo.Name, null);
                else
                    RenderInputCheckbox(builder, Metadata.GetDisplayName(), null);
            }
            else if (elementType.EqualNoCase("file"))
            {
                if (_metadataAttribute.CustomRenderMode == CustomRenderMode.Enabled)
                    RenderCustomInputFile(builder);
                else
                    RenderInputFile(builder, null);
            }
            else
            {
                RenderElement(builder, elementName, elementType);
            }

            return builder;
        }

        /// <summary>
        /// Renders an HTML element to the <paramref name="builder"/> render output.
        /// </summary>
        /// <param name="builder">A <see cref="NestedTagBuilder"/> that will receive the render output.</param>
        /// <param name="elementName">The name of the HTML element to render.</param>
        /// <param name="elementType">The type of the HTML element to render.</param>
        public virtual void RenderElement(NestedTagBuilder builder, string elementName, string elementType)
        {
            var elementBuilder = NestedTagBuilder.Create(elementName)
                .AddMultipleAttributes(AdditionalAttributes);

            if (elementType != null && elementName.EqualNoCase("input"))
            {
                elementBuilder.AddAttributeIf(elementType.EqualNoCase("number"), "step", _stepAttributeValue);

                // 'text' is the default attribute type; don't add if elementType == 'text'
                elementBuilder.AddAttributeIf(!elementType.EqualNoCase("text"), "type", elementType);
            }

            var data = Metadata;
            var attr = data.Attribute;

            elementBuilder
                .AddClass(CssClass)
                .AddAttributeIfNotBlank("id", _inputId)
                .AddAttributeIfNotBlank("title", data.GetDisplayString(attr.Description))
                .AddAttributeIfNotBlank("placeholder", data.GetDisplayString(attr.Prompt))
                .AddAttributeIf(_renderOptions?.GenerateNameAttribute ?? true, "name", data.PropertyInfo.Name)
                ;

            CheckDisabled(elementBuilder);

            //if (_propertyType.IsString())
            //    elementBuilder.AddAttribute("value", FormatValueAsString(Value));
            //else
                elementBuilder.AddAttribute("value", BindConverter.FormatValue(Metadata.PropertyInfo.PropertyType, Value)?.ToString());

            if (elementName.EqualNoCase("select"))
                RenderSelectOptions(elementBuilder);

            builder.AddContent(elementBuilder.ToString());
        }

        /// <summary>
        /// Formats the value as a string. Derived classes can override this to determine 
        /// the formating used for <see cref="CurrentValueAsString"/>.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>A string representation of the value.</returns>
        protected virtual string FormatValueAsString(object value)
        {
            switch (value)
            {
                case DateTime dateTimeValue:
                    return BindConverter.FormatValue(dateTimeValue, _format, _culture);
                case DateTimeOffset dateTimeOffsetValue:
                    return BindConverter.FormatValue(dateTimeOffsetValue, _format, _culture);
                default:
                    return value?.ToString();
            }
        }

        /// <summary>
        /// Determines the frame to generate representing an HTML element.
        /// </summary>
        /// <param name="elementType">Returns the type of element to generate.</param>
        /// <returns></returns>
        public virtual string GetElement(out string elementType)
        {
            string element;

            elementType = _metadataAttribute.UITypeHint.IsWhiteSpace()
                ? null
                : _metadataAttribute.UITypeHint;

            if (_metadataAttribute.UIHint.IsNotWhiteSpace())
                element = _metadataAttribute.UIHint;
            else
                element = "input";

            if (elementType == null)
            {
                var pi = _metadataAttribute.GetProperty();
                var dataType = pi.GetCustomAttribute<DataTypeAttribute>(true);

                if (dataType != null)
                    elementType = dataType.GetControlType();
                else if (pi.GetCustomAttribute<EmailAddressAttribute>(true) != null)
                    elementType = "email";
                else
                    // check for other custom attributes that might 
                    // give us a hint about the type of control to render
                    elementType = pi.PropertyType.GetControlType();

                if (elementType.IsWhiteSpace() && SupportsInputDate())
                    elementType = "date";
            }

            return element;
        }

        /// <summary>
        /// Renders option elements for a select element to the supplied <see cref="NestedTagBuilder"/>.
        /// </summary>
        /// <param name="builder">A <see cref="NestedTagBuilder"/> that will receive the render output.</param>
        /// <returns>An integer that represents the next position of the instruction in the source code.</returns>
        public virtual void RenderSelectOptions(NestedTagBuilder builder)
        {
            var propertyInfo = Metadata.PropertyInfo;
            var options = (_renderOptions?.OptionsGetter?.Invoke(propertyInfo.Name) ?? Metadata.Options)?.ToList();

            if (options?.Count > 0)
            {
                var selectedValue = $"{Value}";
                var promptId = string.Empty;
                var prompt = Metadata.GetDisplayString(_metadataAttribute.Prompt);
                var defaultOption = options.Where(opt => opt.IsPrompt).FirstOrDefault();

                if (defaultOption != null)
                {
                    prompt = defaultOption.Value;
                    promptId = defaultOption.Id;

                    options.Remove(defaultOption);
                }

                if (promptId.IsWhiteSpace() && (_nullableUnderlyingType ?? _propertyType).IsNumeric())
                    promptId = "0";

                __createOption(promptId, prompt);

                foreach (var item in options)
                {
                    __createOption(item.Id, item.Value);
                }

                void __createOption(string __id, string __value)
                {
                    builder.AddChild(NestedTagBuilder.Create("option")
                        .AddAttribute("value", __id)
                        .AddAttributeIf(string.Equals(selectedValue, __id), "selected")
                        .AddContent(__value)
                    );
                }
            }
            else
            {
                //builder.AddContent(ChildContent);
            }
        }

        /// <summary>
        /// Renders a collection of input elements of type 'radio' to the supplied <see cref="NestedTagBuilder"/>.
        /// </summary>
        /// <param name="builder">A <see cref="NestedTagBuilder"/> that will receive the render output.</param>
        /// <returns>An integer that represents the next position of the instruction in the source code.</returns>
        public virtual void RenderRadioOptions(NestedTagBuilder builder)
        {
            var propertyName = Metadata.PropertyInfo.Name;
            var options = _renderOptions?.OptionsGetter?.Invoke(propertyName) ?? Metadata.Options;

            if (options?.Any() == true)
            {
                var customRadio = _metadataAttribute.CustomRenderMode == CustomRenderMode.Enabled;

                foreach (var item in options)
                {
                    if (customRadio)
                    {
                        RenderCustomFormCheck(
                            /* builder */ builder, 
                            /* radio */ true, 
                            /* label */ item.Value, 
                            /* name */ propertyName, 
                            /* value */ item.Id);
                    }
                    else
                    {
                        RenderInputRadio(
                            /* builder */ builder,
                            /* propertyName */ propertyName,
                            /* value */ item.Id,
                            /* additionalCssClass */ null,
                            /* label */ item.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Renders a checkbox to the supplied <see cref="NestedTagBuilder"/>.
        /// </summary>
        /// <param name="builder">A <see cref="NestedTagBuilder"/> that will receive the render output.</param>
        /// <param name="additionalCssClass">The custom CSS class to add to the existing <see cref="CssClass"/>.</param>
        /// <param name="label">The text of the associated label. Should be null if you already wrapped the input inside a label.</param>
        public virtual void RenderInputCheckbox(NestedTagBuilder builder, string additionalCssClass = null, string label = null)
        {
            bool insideLabel = label.IsNotWhiteSpace();
            var labelBuilder = insideLabel ? NestedTagBuilder.Create("label") : null;

            var inputBuilder = NestedTagBuilder.Create("input")
                .AddMultipleAttributes(AdditionalAttributes)
                .AddAttribute("type", "checkbox")
                .AddClass($"{additionalCssClass} {CssClass}".Trim())
                .AddAttributeIfNotBlank("id", _inputId)
                .AddAttributeIf(_renderOptions?.GenerateNameAttribute ?? true, "name", Metadata.PropertyInfo.Name);

            CheckDisabled(inputBuilder);

            inputBuilder.AddAttributeIf(((bool?)Value) ?? false, "checked").AddAttribute("value", "true");
            
            if (insideLabel)
            {
                labelBuilder
                    .AddContent("&nbsp;")
                    .AddContent(label)
                    .AddChild(inputBuilder);

                builder.AddContent(labelBuilder.ToString());
            }
            else
            {
                builder.AddContent(inputBuilder.ToString());
            }
        }

        /// <summary>
        /// Renders an input radio to the supplied <see cref="NestedTagBuilder"/>.
        /// </summary>
        /// <param name="builder">A <see cref="NestedTagBuilder"/> that will receive the render output.</param>
        /// <param name="propertyName">The name of the input radio.</param>
        /// <param name="value">The value of the radio input.</param>
        /// <param name="additionalCssClass">The custom CSS class to add to the existing <see cref="CssClass"/>.</param>
        /// <param name="label">The text of the associated label. Should be null if you already wrapped the input inside a label.</param>
        public virtual void RenderInputRadio(NestedTagBuilder builder, string propertyName, object value, string additionalCssClass = null, string label = null)
        {
            /*
            <label>
                <input type="radio" class="form-check-input" name="@propertyName" value="@item.Id" @onchange="HandleChange" checked="@IsChecked" />
                @item.Value
            </label>
            */
            var insideLabel = label.IsNotWhiteSpace();
            var labelBuilder = insideLabel ? NestedTagBuilder.Create("label") : null;

            var inputBuilder = NestedTagBuilder.Create("input")
                .AddAttribute("type", "radio")
                .AddAttribute("value", FormatValueAsString(value))
                .AddClass($"{additionalCssClass} {CssClass}".Trim())
                .AddAttributeIf(_renderOptions?.GenerateNameAttribute ?? true, "name", propertyName);

            CheckDisabled(inputBuilder);

            inputBuilder.AddAttributeIf(Equals(Value, value), "checked");
            
            if (insideLabel)
            {
                builder.AddChild(labelBuilder
                    .AddContent("&nbsp;")
                    .AddContent(label)
                    .AddChild(inputBuilder)
                );
            }
            else
            {
                builder.AddChild(inputBuilder);
            }
        }

        /// <summary>
        /// Renders an input file to the supplied <see cref="NestedTagBuilder"/>.
        /// </summary>
        /// <param name="builder">A <see cref="NestedTagBuilder"/> that will receive the render output.</param>
        /// <param name="additionalCssClass">The custom CSS class to add to the existing <see cref="CssClass"/>.</param>
        public virtual void RenderInputFile(NestedTagBuilder builder, string additionalCssClass = null)
        {
            var inputBuilder = NestedTagBuilder.Create("input")
                .AddMultipleAttributes(AdditionalAttributes)
                .AddClass($"{additionalCssClass} {CssClass}".Trim())
                .AddAttribute("type", "file")
                .AddAttributeIfNotBlank("id", _inputId)
                .AddAttributeIf(_renderOptions?.GenerateNameAttribute ?? true, "name", Metadata.PropertyInfo.Name);

            if (_metadataAttribute.GetFileAttribute() != null)
                AddInputFileAttributes(inputBuilder, _metadataAttribute.GetFileAttribute());

            builder.AddChild(inputBuilder);
        }

        /// <summary>
        /// Adds attributes to an input file from the custom attribute <see cref="InputFileAttribute"/>.
        /// </summary>
        /// <param name="builder">A <see cref="NestedTagBuilder"/> that will receive the render output.</param>
        /// <param name="fileAttr">The <see cref="InputFileAttribute"/> to check.</param>
        public virtual void AddInputFileAttributes(NestedTagBuilder builder, InputFileAttribute fileAttr)
        {
            if (fileAttr == null) throw new ArgumentNullException(nameof(fileAttr));
            
            builder
                .AddAttributeIfNotBlank("accept", fileAttr.Accept)
                .AddAttributeIf(fileAttr.Multiple, "multiple");
        }

        /// <summary>
        /// Renders to the supplied <see cref="NestedTagBuilder"/> an input of type file.
        /// </summary>
        /// <param name="builder">A <see cref="NestedTagBuilder"/> that will receive the render output.</param>
        public virtual void RenderCustomInputFile(NestedTagBuilder builder)
        {
            /*
            <div class="custom-file">
                <input type="file" class="custom-file-input" id="@id" name="@(Name ?? id)" title="@Text">
                <label class="custom-file-label" for="@id">@Prompt</label>
            </div>
            */
            var div = NestedTagBuilder.Create("div").AddClass("custom-file");

            RenderInputFile(
                /*builder */ div, 
                /* additionalCssClass */ "custom-file-input");

            builder.AddContent(
                div.AddChild(
                    NestedTagBuilder.Create("label").AddClass("custom-file-label")
                    .AddAttributeIfNotBlank("for", _inputId)
                    .AddContent(Metadata.GetDisplayString(_metadataAttribute.Prompt) ?? Metadata.GetDisplayName())
                ).ToString()
            );
        }

        /// <summary>
        /// Renders to the specified <see cref="NestedTagBuilder"/> a checkbox 
        /// or a radio input inside a &lt;div class="form-check"> wrapper element.
        /// </summary>
        /// <param name="builder">A <see cref="NestedTagBuilder"/> that will receive the render output.</param>
        /// <param name="radio">true to render a radio input; otherwise, false to render a checkbox input.</param>
        /// <param name="label">The label text.</param>
        /// <param name="name">The name of the input radio.</param>
        /// <param name="value">The value of the radio input.</param>
        public virtual void RenderCustomFormCheck(NestedTagBuilder builder, bool radio, string label, string name = null, string value = null)
        {
            /* This is a variation of what we're building:
            <div class="form-check">
                <label class="form-check-label">
                    <input type="radio" class="form-check-input" name="@propertyName" value="@item.Id" @onchange="HandleChange" checked="@IsChecked" />
                    @item.Value
                </label>
            </div>
            */
            const string FORM_CHECK_INPUT = "form-check-input";

            var div = NestedTagBuilder.Create("div").AddClass("form-check");
            var ntbLabel = NestedTagBuilder.Create("label").AddClass("form-check-label");

            if (radio)
            {
                RenderInputRadio(
                    /* builder */ ntbLabel, 
                    /* propertyName */ name, 
                    /* value */ value, 
                    /* additionalCssClass */ FORM_CHECK_INPUT,
                    /* label */ null);

                ntbLabel.AddChild(NestedTagBuilder.Create("span").SetInnerHtml(label));
            }
            else
            {
                RenderInputCheckbox(
                    /* builder */ntbLabel, 
                    /* additionalCssClass */ FORM_CHECK_INPUT,
                    /* label */ null);

                ntbLabel.AddContent(label);
            }

            builder.AddChild(div.AddChild(ntbLabel));
        }

        /// <summary>
        /// Determines at run-time the disabled state of the current <see cref="AutoInputBase"/>
        /// if the associated property was statically-marked as disabled with the property
        /// <see cref="FormAttributeBase.Disabled"/> set to true.
        /// </summary>
        /// <param name="builder">A <see cref="NestedTagBuilder"/> that will receive the render output.</param>
        public virtual void CheckDisabled(NestedTagBuilder builder)
        {
            if (_metadataAttribute.Disabled)
            {
                // the design-time state is 'disabled', now check the run-time state
                bool disabled = _renderOptions?.DisabledGetter?.Invoke(_propertyType.Name) ?? true;
                builder.AddAttributeIf(disabled, "disabled");
            }
        }

        #region helpers

        private void CheckIfInputNumber()
        {
            GetElement(out var elementType);

            if (elementType.EqualNoCase("number"))
            {
                var targetType = _nullableUnderlyingType ?? _propertyType;
                if (targetType.SupportsInputNumber())
                    _stepAttributeValue = "any";
                else
                    throw new InvalidOperationException($"The type '{targetType}' is not a supported numeric type.");
            }
        }

        private bool SupportsInputDate() => (_nullableUnderlyingType ?? _propertyType).IsDate();

        private void InitCultureAndFormat()
        {
            if (_metadataAttribute.CultureName.IsNotWhiteSpace())
                _culture = CultureInfo.GetCultureInfo(_metadataAttribute.CultureName);
            else
                _culture = CultureInfo.CurrentCulture;

            if (_metadataAttribute.Format.IsNotWhiteSpace())
                _format = _metadataAttribute.Format;
            else if (SupportsInputDate())
                _format = "yyyy-MM-dd"; // Compatible with HTML date inputs
        }

        #endregion
    }
}
