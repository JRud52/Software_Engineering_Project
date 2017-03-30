using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Software_Engineering_Project.App_Code
{
    public static class Extensions
    {
        public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, object routeValues = null, AjaxOptions ajaxOptions = null, object htmlAttributes = null)
        {
            var repID = Guid.NewGuid().ToString();
            var link = ajaxHelper.ActionLink(repID, actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
            return MvcHtmlString.Create(link.ToString().Replace(repID, linkText));
        }

        public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions)
        {
            var repID = Guid.NewGuid().ToString();
            var link = ajaxHelper.ActionLink(repID, actionName, controllerName, ajaxOptions);
            return MvcHtmlString.Create(link.ToString().Replace(repID, linkText));
        }

    }
}