using Carfamsoft.ModelToView.Shared;
using Carfamsoft.ModelToView.ViewAnnotations;

namespace AutoRazorViewModels
{
    public class AutoInputMetadataBag
    {
        public object ViewModel { get; set; }
        public AutoInputMetadata Metadata { get; set; }
        public ControlRenderOptions RenderOptions { get; set; }
        public ContentAlignment LabelAlignment { get; set; } = ContentAlignment.Left;
        public object LabelContent { get; set; }
        public object ChildContent { get; set; }
        public object ValidationContent { get; set; }
    }
}