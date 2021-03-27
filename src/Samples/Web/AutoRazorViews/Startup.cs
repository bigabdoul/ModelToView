using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Carfamsoft.ModelToView.Samples.Web.AutoRazorViews.Startup))]
namespace Carfamsoft.ModelToView.Samples.Web.AutoRazorViews
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
