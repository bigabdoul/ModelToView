using Carfamsoft.ModelToView.Shared;
using Carfamsoft.ModelToView.ViewAnnotations;
using Carfamsoft.ModelToView.WebPages;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Carfamsoft.ModelToView.Mvc
{
    /// <summary>
    /// Contains extension methods for an instance of the <see cref="string"/> class.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Renders the specified model as an HTML-encoded string that should not be encoded again.
        /// </summary>
        /// <param name="_">The unused <see cref="HtmlHelper"/>.</param>
        /// <param name="model">The model to render.</param>
        /// <param name="ngModel">
        /// The model prefix for the AngularJS ng-model attribute.
        /// If the value is null the ng-mnodel attribute is not added.
        /// If the value is empty the attribute is added without the prefix.
        /// </param>
        /// <returns>An HTML-encoded string that should not be encoded again.</returns>
        public static IHtmlString RenderAsHtml(this HtmlHelper _, object model, string ngModel = null)
        {
            return MvcHtmlString.Create(NestedTagBuilder.Create("div").RenderAsHtmlString(model, ngModel));
        }

        /// <summary>
        /// Renders an instance of the specified type <typeparamref name="T"/> as an HTML string.
        /// </summary>
        /// <typeparam name="T">The model type.</typeparam>
        /// <param name="_">The unused <see cref="HtmlHelper"/>.</param>
        /// <param name="options">The options for rendering the object of type <typeparamref name="T"/>.</param>
        /// <returns>An HTML-encoded string that should not be encoded again.</returns>
        [Obsolete]
        public static IHtmlString RenderAsHtml<T>(this HtmlHelper _, ControlRenderOptions options = null) where T : new()
        {
            return MvcHtmlString.Create(new ObjectHtmlViewEngine(new T(), options).Render());
        }

        /// <summary>
        /// Renders the specified model as an HTML form-encoded string that should not be encoded again.
        /// </summary>
        /// <param name="helper">The used HtmlHelper.</param>
        /// <param name="model">The model to render.</param>
        /// <param name="ngModel">
        /// The model prefix for the AngularJS ng-model attribute.
        /// If the value is null the ng-mnodel attribute is not added.
        /// If the value is empty the attribute is added without the prefix.
        /// </param>
        /// <param name="formName">The name of the form to use. If not specified, a randomly-generated name will be used.</param>
        /// <param name="ngSubmitFunctionName">The name of the AngularJS function used to submit the form.</param>
        /// <param name="submitText">The text of the submit button, if included.</param>
        /// <param name="submitUrl">The URL of the form action. Is ignored if <paramref name="ngModel"/> is specified.</param>
        /// <param name="omitSubmitButton">true to omit the submit button, otherwise, false.</param>
        /// <param name="additionalFormAttributes">Additional form attributes to include.</param>
        /// <param name="additionalHtml">Additional HTML content to include into the form.</param>
        /// <param name="generateNameAttributes">true to generate the 'name' atribute for rendrered form controls.</param>
        /// <returns>An HTML-encoded string that should not be encoded again.</returns>
        public static IHtmlString AutoEditForm(this HtmlHelper helper,
            object model,
            string ngModel = null,
            string formName = null,
            string ngSubmitFunctionName = "submitForm()",
            string submitText = "Submit",
            string submitUrl = null,
            bool omitSubmitButton = true,
            IDictionary<string, string> additionalFormAttributes = null,
            IHtmlString additionalHtml = null,
            bool generateNameAttributes = false)
        {
            var form = NestedTagBuilder.Create("form");
            var attributes = form.Attributes;

            if (ngModel.IsNotWhiteSpace())
            {
                if (formName.IsWhiteSpace())
                    formName = $"ngform_{Guid.NewGuid().GetHashCode():x}";

                attributes.Add("name", formName);

                if (ngSubmitFunctionName.IsNotWhiteSpace())
                    attributes.Add("ng-submit", $"{formName}.$valid && {ngSubmitFunctionName}");

                attributes.Add("novalidate", "novalidate");
            }
            else if (submitUrl.IsNotWhiteSpace())
            {
                attributes.Add("action", submitUrl);
                attributes.Add("method", "post");
                attributes.Add("enctype", "multipart/form-data");
            }

            if (additionalFormAttributes?.Count > 0)
            {
                foreach (var kvp in additionalFormAttributes)
                    attributes[kvp.Key] = kvp.Value;
            }

            var renderOptions = new ControlRenderOptions
            {
                GenerateNameAttribute = generateNameAttributes,
                GenerateIdAttribute = true,
                CamelCaseId = true
            };

            form.Render(model, ngModel, renderOptions);

            if (additionalHtml != null)
                form.AddChild(NestedTagBuilder.Create("fieldset").AddContent(additionalHtml.ToHtmlString()));

            if (!omitSubmitButton)
            {
                var button = NestedTagBuilder.Create("button")
                    .AddAttribute("type", "submit")
                    .AddClass("btn btn-primary")
                    .AddContent(submitText);
                form.AddChild(button);
            }

            form.AddChild(NestedTagBuilder.Create("fieldset").AddContent(helper.AntiForgeryToken().ToHtmlString()));

            return MvcHtmlString.Create(form.ToString());
        }

        /// <summary>
        /// Renders the specified model as an HTML-encoded string that should not be encoded again.
        /// </summary>
        /// <param name="_">The unused <see cref="HtmlHelper"/>.</param>
        /// <param name="viewModel">The view model to render.</param>
        /// <param name="formAttrs">An object that encapsulates form attribute values.</param>
        /// <param name="renderOptions">An object that controls a part of the HTML generation process.</param>
        /// <returns>An HTML-encoded string that should not be encoded again.</returns>
        public static IHtmlString AutoEditForm(this HtmlHelper _,
                                               object viewModel,
                                               FormAttributes formAttrs = null,
                                               ControlRenderOptions renderOptions = null)
        {
            return MvcHtmlString.Create(formAttrs.RenderAutoEditForm(viewModel, renderOptions).ToString());
        }

        /// <summary>
        /// Renders the specified <paramref name="metadata"/> to the render output.
        /// </summary>
        /// <param name="metadata">The metadata to render.</param>
        /// <param name="viewModel">An object used to obtain the value of the associated property defined in <paramref name="metadata"/>.</param>
        /// <returns>An HTML-encoded string that should not be encoded again.</returns>
        /// <param name="options">An object that controls a part of the HTML generation process.</param>
        public static IHtmlString Render(this AutoInputMetadata metadata, object viewModel, ControlRenderOptions options = null)
        {
            var output = NestedTagBuilder.Create("div").RenderAutoInputBase(metadata, viewModel, options);
            return MvcHtmlString.Create(output.GetInnerHtml());
        }
    }
}
