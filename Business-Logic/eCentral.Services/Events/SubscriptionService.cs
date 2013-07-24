using System.Collections.Generic;
using eCentral.Core.Infrastructure;

namespace eCentral.Services.Events
{
    public class SubscriptionService : ISubscriptionService
    {
        public IList<IUser<T>> GetSubscriptions<T>()
        {
            return EngineContext.Current.ResolveAll<IUser<T>>();
        }
    }
}
