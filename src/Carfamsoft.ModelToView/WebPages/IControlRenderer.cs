using Carfamsoft.ModelToView.Shared;

namespace Carfamsoft.ModelToView.WebPages
{
    /// <summary>
    /// Specifies the contract required to render an instance of the <see cref="ControlInfo"/> class.
    /// </summary>
    [System.Obsolete]
    public interface IControlRenderer
    {
        /// <summary>
        /// Gets or sets the options for rendering an object as a collection of HTML controls.
        /// </summary>
        ControlRenderOptions RenderOptions { get; set; }

        /// <summary>
        /// Renders the specified control info.
        /// </summary>
        /// <param name="info">The control info to render.</param>
        /// <returns></returns>
        string Render(ControlInfo info);
    }
}