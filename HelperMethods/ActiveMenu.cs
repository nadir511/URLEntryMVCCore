using Microsoft.AspNetCore.Mvc.Rendering;

namespace URLEntryMVC.HelperMethods
{
    public static class ActiveMenu
    {
        public static string IsActive(this IHtmlHelper html,string pointCategory, string controller = null, string action = null, string cssClass = null)
        {
            var routeData = html.ViewContext.RouteData;
            var routeData2 = html.ViewContext.RouteData.Values;
            var routeAction = routeData.Values["action"].ToString();
            var routeController = routeData.Values["controller"].ToString();
            if (routeData.Values.ContainsKey("pointCategory"))
            {
                var pointCat = routeData.Values["pointCategory"];
                var test = pointCat;
            }

               

            var returnActive = (controller == routeController && (action == routeAction || routeAction == "Details"));
            return returnActive ? "active" : "";
        }
    }
}
