using System.Collections.Generic;
using eCentral.Core.Domain.Security;

namespace eCentral.Services.Security
{
    public interface IPermissionProvider
    {
        IEnumerable<PermissionRecord> GetPermissions();
        IEnumerable<DefaultPermissionRecord> GetDefaultPermissions();
    }
}
