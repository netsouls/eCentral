using System.Collections.Generic;

namespace eCentral.Services.Events
{
    public interface ISubscriptionService
    {
        IList<IUser<T>> GetSubscriptions<T>();
    }
}
