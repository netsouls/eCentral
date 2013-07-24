
namespace eCentral.Services.Events
{
    public interface IUser<T>
    {
        void HandleEvent(T eventMessage);
    }
}
