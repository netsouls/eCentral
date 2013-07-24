using System;
using System.Web;
using System.Web.Mvc;
using eCentral.Core;
using ServiceStack.Text;

namespace eCentral.Web.Framework.Mvc
{
    /// <summary>
    /// JsonResult using ServiceStack's JsonSerializer
    /// </summary>
    public class ServiceStackJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            Guard.IsNotNull(context, "context");

            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("JSON Get Request Not Allowed");
            }

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                response.Write(JsonSerializer.SerializeToString(Data));
            }
        }
    }
}
