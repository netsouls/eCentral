using System;
using System.Collections.Generic;
using System.Web.Mvc;
using eCentral.Core.Infrastructure;

namespace eCentral.Web.Framework.Mvc
{
    public class SiteDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            try
            {
                return EngineContext.Current.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                var type = typeof(IEnumerable<>).MakeGenericType(serviceType);
                return (IEnumerable<object>)EngineContext.Current.Resolve(type);
            }
            catch
            {
                return null;
            }

        }
    }
}
