using Microsoft.AspNetCore.Mvc.Rendering;

namespace URLEntryMVC.HelperMethods
{
    public static class ActiveMenu
    {
        public static string IsActive(this IHtmlHelper html, string controller = null, string action = null, string cssClass = null)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = routeData.Values["action"].ToString();
            var routeController = routeData.Values["controller"].ToString();

            var returnActive = (controller == routeController && (action == routeAction || routeAction == "Details"));
            return returnActive ? "active" : "";
        }
    }
}
