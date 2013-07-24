
namespace eCentral.Services.Installation
{
    public partial interface IInstallationService
    {
        void InstallMessageTemplates();

        void InstallActivityLogTypes();

        void InstallPermissions();
    }
}
