using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;

namespace UTM.Keto.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.User.Identity is FormsIdentity formsIdentity)
                {
                    // Получаем билет аутентификации из куки
                    FormsAuthenticationTicket ticket = formsIdentity.Ticket;
                    
                    // Разбиваем данные пользователя
                    string[] userData = ticket.UserData.Split('|');
                    if (userData.Length >= 3)
                    {
                        // Создаем массив ролей (в данном случае, одну роль)
                        string[] roles = { userData[2] };
                        
                        // Устанавливаем принципал с ролями
                        HttpContext.Current.User = new GenericPrincipal(formsIdentity, roles);
                    }
                }
            }
        }
    }
}