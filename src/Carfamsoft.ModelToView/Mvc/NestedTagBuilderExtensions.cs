using Carfamsoft.ModelToView.Shared;
using Carfamsoft.ModelToView.Shared.Extensions;
using Carfamsoft.ModelToView.ViewAnnotations;
using Carfamsoft.ModelToView.WebPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Carfamsoft.ModelToView.Mvc
{
    /// <summary>
    /// Provides extension methods for instances of the <see cref="NestedTagBuilder"/> class.
    /// </summary>
    public static class NestedTagBuilderExtensions
    {
        /// <summary>
        /// Renders the <paramref name="builder"/> and removes the outer tag from the output.
        /// </summary>
        /// <param name="builder">The tag builder from which to remove the outer tag.</param>
        /// <returns></returns>
        public static string GetInnerHtml(this NestedTagBuilder builder)
        {
            if (HtmlTagInfo.IsSelfClosing(builder.TagName))
                // simply returning builder.InnerHtml is not going to render its child tags
                return builder.ToString();
            
            var sb = new System.Text.StringBuilder(builder.ToString());
            const int BRACKETS = 2; // <>
            var tagLength = builder.TagName.Length + BRACKETS;
            var closingTagLength = tagLength + 1; // the slash like in </div>

            sb.Remove(0, tagLength).Remove(sb.Length - closingTagLength, closingTagLength);
            return sb.ToString();
        }

        /// <summary>
        /// Renders the specified model as an HTML string.
        /// </summary>
        /// <param name="builder">The <see cref="NestedTagBuilder"/> that will contain the render output.</param>
        /// <param name="viewModel">The view model to render.</param>
        /// <param name="ngModel">
        /// The model prefix for the AngularJS ng-model attribute.
        /// If the value is null the ng-mnodel attribute is not added.
        /// If the value is empty the attribute is added without the prefix.
        /// </param>
        /// <param name="renderOptions">An object that controls a part of the HTML generation process.</param>
        /// <returns></returns>
        public static string RenderAsHtmlString(
            this NestedTagBuilder builder,
            object viewModel,
            string ngModel = null,
            ControlRenderOptions renderOptions = null)
        {
            return builder.Render(viewModel, ngModel, renderOptions).ToString();
        }

        /// <summary>
        /// Renders the specified model to the given <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="NestedTagBuilder"/> that will contain the render output.</param>
        /// <param name="viewModel">The view model to render.</param>
        /// <param name="ngModel">
        /// The model prefix for the AngularJS ng-model attribute.
        /// If the value is null the ng-mnodel attribute is not added.
        /// If the value is empty the attribute is added without the prefix.
        /// </param>
        /// <param name="renderOptions">An object that controls a part of the HTML generation process.</param>
        /// <returns></returns>
        public static NestedTagBuilder Render(this NestedTagBuilder builder,
            object viewModel,
            string ngModel = null,
            ControlRenderOptions renderOptions = null)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            if (renderOptions == null)
                renderOptions = new ControlRenderOptions { CamelCaseId = true };

            if (ngModel != null && renderOptions.AdditionalAttributesGetter == null)
            {
                renderOptions.AdditionalAttributesGetter = propertyName =>
                {
                    var modelName = renderOptions.CamelCaseId ? propertyName.ToCamelCase() : propertyName;

                    if (ngModel.Length > 0)
                        modelName = $"{ngModel}.{modelName}";

                    return new Dictionary<string, object> { { "ng-model", modelName } };
                };
            }

            if (viewModel.ExtractMetadata(out var groups))
            {
                foreach (var group in groups)
                {
                    builder.RenderFormDisplayGroup(group, viewModel, renderOptions);
                }
            }

            return builder;
        }

        /// <summary>
        /// Renders the specified model as an HTML string.
        /// </summary>
        /// <param name="formAttrs">An object that encapsulates form attribute values.</param>
        /// <param name="viewModel">The view model to render.</param>
        /// <param name="renderOptions">A function that supplies additional attributes to the render output.</param>
        /// <returns>An initialized instance of the <see cref="NestedTagBuilder"/> class representing the form.</returns>
        public static NestedTagBuilder RenderAutoEditForm(this FormAttributes formAttrs,
                                              object viewModel,
                                              ControlRenderOptions renderOptions = null)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            var form = CreateForm(formAttrs);

            if (renderOptions == null)
                renderOptions = ControlRenderOptions.CreateDefault();

            if (viewModel.ExtractMetadata(out var groups))
            {
                foreach (var group in groups)
                {
                    form.RenderFormDisplayGroup(group, viewModel, renderOptions);
                }
            }

            return form;
        }

        /// <summary>
        /// Renders the specified <paramref name="group"/> into an 
        /// instance of <see cref="NestedTagBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="NestedTagBuilder"/> that will contain the render output.</param>
        /// <param name="group">The grouped collection of <see cref="AutoInputMetadata"/> used to render <paramref name="viewModel"/>.</param>
        /// <param name="viewModel">The view model to render.</param>
        /// <param name="renderOptions">An object that controls a part of an HTML generation process.</param>
        public static NestedTagBuilder RenderFormDisplayGroup(this NestedTagBuilder builder,
                                  FormDisplayGroupMetadata group,
                                  object viewModel, 
                                  ControlRenderOptions renderOptions = null)
        {
            /* Sample render output (Razor View)
             <fieldset class="display-group">
                @if (group.ShowName)
                {
                    <legend>@group.Name</legend>
                }
                <div class="display-group-body @group.CssClass">
                    @foreach (AutoInputMetadata data in group.Items)
                    {
                        if (data.Attribute.Ignore) continue;
                        <div class="@data.Attribute.ColumnCssClass">
                            <AutoInput Metadata="data">@ChildContent</AutoInput>
                        </div>
                    }
                </div>
            </fieldset>
             */
            return builder.AddChild(NestedTagBuilder.Create("fieldset").AddClass("display-group")
                .AddContentIf(group.ShowName, () => NestedTagBuilder.Create("legend").AddContent(group.Name).ToString())
                .AddContent
                (
                    NestedTagBuilder.Create("div").AddClass("display-group-body").AddClass(group.CssClass)
                    .ForEach
                    (
                        group.Items.Where(meta => !meta.Attribute.IsIgnored()),
                        (data, displayGroupBody) =>
                        {
                            displayGroupBody.AddChild(
                                NestedTagBuilder.Create("div")
                                .AddClass(data.Attribute.ColumnCssClass)
                                .RenderAutoInput(data, viewModel, renderOptions: renderOptions)
                            );
                        }
                    ).ToString()
                ));
        }

        /// <summary>
        /// Renders a button to the specified <paramref name="builder"/> children's collection.
        /// </summary>
        /// <param name="builder">The <see cref="NestedTagBuilder"/> that will contain the render output.</param>
        /// <param name="text">The button text.</param>
        /// <param name="type">The type of button to render.</param>
        /// <param name="cssClass">The CSS class to add to the button.</param>
        /// <param name="config">An action used to perform more configuration on the button.</param>
        /// <returns>A reference to <paramref name="builder"/>.</returns>
        public static NestedTagBuilder RenderButton(
            this NestedTagBuilder builder,
            string text = null,
            string type = "submit",
            string cssClass = "btn btn-primary",
            Action<NestedTagBuilder> config = null)
        {
            var button = NestedTagBuilder.Create("button");

            builder.AddChild(button);

            config?.Invoke(button);

            button.AddAttribute("type", type)
                .AddClass(cssClass)
                .AddChild(NestedTagBuilder.Create("span").AddContent(text));

            return builder;
        }

        
        internal static NestedTagBuilder CreateForm(FormAttributes attrs = null)
        {
            if (attrs == null) attrs = new FormAttributes();

            var form = NestedTagBuilder.Create("form");

            form.AddAttributeIfNotBlank("id", attrs.Id)
                .AddAttributeIfNotBlank("name", attrs.Name)
                .AddAttributeIfNotBlank("action", attrs.Action)
                .AddAttributeIfNotBlank("method", attrs.Method)
                .AddAttributeIfNotBlank("enctype", attrs.EncType);

            foreach (var kvp in attrs.AdditionalAttributes)
                form.MergeAttribute(kvp.Key, kvp.Value);

            return form;
        }

        internal static NestedTagBuilder RenderAutoInput(
            this NestedTagBuilder builder,
            AutoInputMetadata metadata,
            object viewModel,
            NestedTagBuilder labelContent = null, ControlRenderOptions renderOptions = null)
        {
            var attr = metadata.Attribute;
            var propertyName = metadata.PropertyInfo.Name;

            if (true == renderOptions?.GenerateIdAttribute)
            {
                renderOptions.UniqueId = propertyName.GenerateId(renderOptions.CamelCaseId);
            }
            
            if (attr.IsInputRadio || metadata.IsInputCheckbox)
            {
                /*
                 <div class="form-group">
                    @if (!string.IsNullOrWhiteSpace(labelText) && attr.IsInputRadio)
                    {
                        <label class="control-label">@labelText</label>
                    }
                    <AutoInputBase Metadata="metadata" @bind-Value="metadata.Value" Id="@inputId" class="@attr.InputCssClass" title="@attr.Description" />
                    <AutoValidationMessage Model="metadata.Model" Property="@propertyName" />
                </div>
                 */
                builder.RenderCheckboxOrRadio(metadata, viewModel, renderOptions);
            }
            else
            {
                /*
                 <InputGroupContainer Id="@inputId" LabelText="@labelText" Icon="@attr.Icon">
                    <ChildContent>
                        <AutoInputBase Metadata="metadata" @bind-Value="metadata.Value" Id="@inputId" class="@attr.InputCssClass" title="@attr.Description" placeholder="@attr.Prompt">
                            @ChildContent
                        </AutoInputBase>
                    </ChildContent>
                    <ValidationContent>
                        <AutoValidationMessage Model="metadata.Model" Property="@propertyName" />
                    </ValidationContent>
                </InputGroupContainer>
                 */
                builder.RenderInputGroup(metadata, viewModel, metadata.GetDisplayName(), labelContent, renderOptions);
            }

            return builder;
        }

        internal static NestedTagBuilder RenderInputGroup(
            this NestedTagBuilder builder,
            AutoInputMetadata metadata,
            object viewModel,
            string labelText = null,
            NestedTagBuilder labelContent = null,
            ControlRenderOptions renderOptions = null)
        {
            /*
             <div class="form-group">
                @if (!string.IsNullOrWhiteSpace(LabelText))
                {
                    <label for="@Id" class="control-label">@LabelText</label>
                }
                else if (LabelContent != null)
                {
                    @LabelContent
                }
                <div class="input-group">
                    @if (!string.IsNullOrWhiteSpace(Icon))
                    {
                        <div class="input-group-prepend">
                            <span class="input-group-text">
                                <i class="@Icon"></i>
                            </span>
                        </div>
                    }
                    @ChildContent
                </div>
                @if (ValidationContent != null)
                {
                    @ValidationContent
                }
            </div>
             */
            return builder.AddChild(NestedTagBuilder.Create("div").AddClass("form-group")
                    .AddChildIf(labelText.IsNotWhiteSpace(),
                        () => NestedTagBuilder.Create("label").AddClass("control-label")
                            .AddAttributeIfNotBlank("for", renderOptions?.UniqueId)
                            .AddContentIfNotBlank(labelText)
                            .AddChildIf(labelContent != null && labelText.IsWhiteSpace(), labelContent)
                    ).AddChild(NestedTagBuilder.Create("div").AddClass("input-group")
                        .AddContentIf(metadata.Attribute.Icon.IsNotWhiteSpace(), () => RenderIcon(metadata.Attribute.Icon))
                        .RenderAutoInputBase(metadata, viewModel, renderOptions)
                    ));
        }

        /// <summary>
        /// Renders the <see cref="AutoInputBase"/> component to the specified <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="NestedTagBuilder"/> that will contain the render output.</param>
        /// <param name="metadata">An object used to render an HTML element.</param>
        /// <param name="viewModel">An object used to obtain the value of the associated property defined in <paramref name="metadata"/>.</param>
        /// <returns>A reference to <paramref name="builder"/>.</returns>
        /// <param name="renderOptions">An object that controls a part of the HTML generation process.</param>
        public static NestedTagBuilder RenderAutoInputBase(
            this NestedTagBuilder builder,
            AutoInputMetadata metadata,
            object viewModel, ControlRenderOptions renderOptions = null)
        {
            return new AutoInputBase(metadata, viewModel, renderOptions)
            {
                AdditionalAttributes = renderOptions?.AdditionalAttributesGetter?.Invoke(metadata.PropertyInfo.Name)
            }.BuildRenderTree(builder);
        }

        internal static string RenderIcon(
            string icon,
            string inputGroupPrependCssClass = "input-group-prepend",
            string inputGroupTextCssClass = "input-group-text")
        {
            return NestedTagBuilder.Create("div").AddClass(inputGroupPrependCssClass)
                .AddChild
                (
                    NestedTagBuilder.Create("span").AddClass(inputGroupTextCssClass)
                    .AddChild(NestedTagBuilder.Create("i").AddClass(icon))
                ).ToString();
        }

        internal static NestedTagBuilder RenderCheckboxOrRadio(
            this NestedTagBuilder builder,
            AutoInputMetadata metadata,
            object viewModel, ControlRenderOptions renderOptions = null)
        {
            /*
             <div class="form-group">
                @if (!string.IsNullOrWhiteSpace(labelText) && attr.IsInputRadio)
                {
                    <label class="control-label">@labelText</label>
                }
                <AutoInputBase Metadata="metadata" @bind-Value="metadata.Value" Id="@inputId" class="@attr.InputCssClass" title="@attr.Description" />
                <AutoValidationMessage Model="metadata.Model" Property="@propertyName" />
            </div>
             */
            var labelText = metadata.GetDisplayName();
            return builder.AddChild(NestedTagBuilder.Create("div").AddClass("form-group")
                .AddChildIf(labelText.IsNotWhiteSpace() && metadata.Attribute.IsInputRadio,
                    () => NestedTagBuilder.Create("label").AddClass("control-label").AddContent(labelText)
                ).RenderAutoInputBase(metadata, viewModel, renderOptions)
            );
        }
    }
}
