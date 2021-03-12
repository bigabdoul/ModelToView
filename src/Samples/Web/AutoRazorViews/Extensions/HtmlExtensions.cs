using Carfamsoft.ModelToView.Shared;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Carfamsoft.ModelToView.Samples.Web.AutoRazorViews.Extensions
{
    /// <summary>
    /// Provides extension methods for instances of the <see cref="HtmlHelper"/> class.
    /// </summary>
    public static class HtmlExtensions
    {
        const string ViewsSharedFolder = "~/Views/Shared/{0}.cshtml";

        /// <summary>
        /// Automatically renders the specified model using a partial view named 
        /// "_AutoEditView" found in the "~/Views/Shared" folder.
        /// </summary>
        /// <typeparam name="T">The type of the model to render.</typeparam>
        /// <param name="html">The <see cref="HtmlHelper"/> used to render the view.</param>
        /// <param name="viewModel">The model to automatically render.</param>
        /// <param name="labelAlignment">Determines how the label is positioned relative to the rendered HTML element.</param>
        /// <returns></returns>
        public static IHtmlString AutoEditView<T>(
            this HtmlHelper html,
            T viewModel,
            ContentAlignment labelAlignment = ContentAlignment.Top,
            ControlRenderOptions renderOptions = null) where T : class, new()
        {
            return html.Partial(
                string.Format(ViewsSharedFolder, "_AutoEditView"), 
                new AutoInputMetadataBag
                { 
                    ViewModel = viewModel ?? new T(), 
                    LabelAlignment = labelAlignment, 
                    RenderOptions = renderOptions 
                });
        }
    }
}
