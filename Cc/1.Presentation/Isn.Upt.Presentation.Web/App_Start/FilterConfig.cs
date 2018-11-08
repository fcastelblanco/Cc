using System.Web.Mvc;
using Isn.Upt.Presentation.ExceptionHandling;

namespace Isn.Upt.Presentation
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
