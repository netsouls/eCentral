using System;
using System.Web;

namespace eCentral.Services.Web
{
    /// <summary>
    /// Defines members that must be implemented for cache the http response
    /// </summary>
    public class HttpResponseCacher : IHttpResponseCacher
    {
        /// <summary>
        /// Caches the response for the specified duration.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="duration">The duration.</param>
        public void Cache(HttpContextBase context, TimeSpan duration)
        {
            if ((duration > TimeSpan.Zero))
            {
                HttpCachePolicyBase cache = context.Response.Cache;

                cache.SetCacheability(HttpCacheability.Public);
                cache.SetOmitVaryStar(true);
                cache.SetExpires(context.Timestamp.Add(duration));
                cache.SetMaxAge(duration);
                cache.SetValidUntilExpires(true);
                cache.SetLastModified(context.Timestamp);
                cache.SetLastModifiedFromFileDependencies();
                cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            }
        }
    }
}
