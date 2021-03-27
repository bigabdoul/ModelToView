using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AutoRazorViewModels.Startup))]
namespace AutoRazorViewModels
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
