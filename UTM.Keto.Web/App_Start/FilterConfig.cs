using System.Web;
using System.Web.Mvc;
using UTM.Keto.Web.Filters;

namespace UTM.Keto.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}