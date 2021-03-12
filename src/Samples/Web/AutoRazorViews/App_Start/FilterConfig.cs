using System.Web;
using System.Web.Mvc;

namespace Carfamsoft.ModelToView.Samples.Web.AutoRazorViews
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
