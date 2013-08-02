using System.Collections.Generic;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;

namespace eCentral.Services.Security
{
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        public static readonly PermissionRecord ViewClients = new PermissionRecord { Name = "View Clients", SystemName = SystemPermissionNames.ViewClients, Category = "Clients" };
        public static readonly PermissionRecord ManageClients = new PermissionRecord { Name = "Manage Clients", SystemName = SystemPermissionNames.ManageClients, Category = "Clients" };

        public static readonly PermissionRecord ViewBranchOffices = new PermissionRecord { Name = "View Offices", SystemName = SystemPermissionNames.ViewBranchOffices, Category = "Offices" };
        public static readonly PermissionRecord ManageBranchOffices = new PermissionRecord { Name = "Manage Offices", SystemName = SystemPermissionNames.ManageBranchOffices, Category = "Offices" };

        public static readonly PermissionRecord ViewCompanies = new PermissionRecord { Name = "View Companies", SystemName = SystemPermissionNames.ViewCompanies, Category = "Company" };
        public static readonly PermissionRecord ManageCompanies = new PermissionRecord { Name = "Manage Companies", SystemName = SystemPermissionNames.ManageCompanies, Category = "Company" };

        public static readonly PermissionRecord AccessDashboard = new PermissionRecord { Name = "Access dashboard", SystemName = SystemPermissionNames.AccessDashboard, Category = "Standard" };
        
        public static readonly PermissionRecord ManageUsers = new PermissionRecord { Name = "Manage Users", SystemName = SystemPermissionNames.ManageUsers, Category = "Users" };
        public static readonly PermissionRecord ManageUsersRoles = new PermissionRecord { Name = "Manage User Roles", SystemName = SystemPermissionNames.ManageUserRoles, Category = "Users" };

        // configurations
        public static readonly PermissionRecord ManageSystemLog    = new PermissionRecord { Name = "Manage System Log", SystemName = SystemPermissionNames.ManageSystemLog, Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageQueue = new PermissionRecord { Name = "Manage Message Queue", SystemName = SystemPermissionNames.ManageMessageQueue, Category = "Configuration" };
        public static readonly PermissionRecord ManageMaintenance  = new PermissionRecord { Name = "Manage Maintenance", SystemName = SystemPermissionNames.ManageMaintenance, Category = "Configuration" };
        public static readonly PermissionRecord ManageMessageTemplates = new PermissionRecord { Name = "Manage Message Templates", SystemName = SystemPermissionNames.ManageMessageTemplates, Category = "Configuration" };
        public static readonly PermissionRecord ManageEmailAccounts = new PermissionRecord { Name = "Manage Email Accounts", SystemName = SystemPermissionNames.ManageEmailAccounts, Category = "Configuration" };
        public static readonly PermissionRecord ManageSettings = new PermissionRecord { Name = "Manage Settings", SystemName = SystemPermissionNames.ManageSettings, Category = "Configuration" };
        public static readonly PermissionRecord ManageCountries = new PermissionRecord { Name = "Manage Countries", SystemName = SystemPermissionNames.ManageCountries, Category = "Configuration" };

        //public static readonly PermissionRecord ManageWidgets = new PermissionRecord { Name = "Manage Widgets", SystemName = SystemPermissionNames.ManageWidgets, Category = "Content Management" };
        //public static readonly PermissionRecord ManageLanguages = new PermissionRecord { Name = "Manage Languages", SystemName = SystemPermissionNames.ManageLanguages, Category = "Configuration" };
        //
        //public static readonly PermissionRecord ManageCurrencies = new PermissionRecord { Name = "Manage Currencies", SystemName = SystemPermissionNames.ManageCurrencies, Category = "Configuration" };
        //public static readonly PermissionRecord ManageActivityLog = new PermissionRecord { Name = "Manage Activity Log", SystemName = SystemPermissionNames.ManageActivityLog, Category = "Configuration" };
        //public static readonly PermissionRecord ManageAcl = new PermissionRecord { Name = "Manage ACL", SystemName = SystemPermissionNames.ManageACL, Category = "Configuration" };
        //public static readonly PermissionRecord ManageSmsProviders = new PermissionRecord { Name = "Manage SMS Providers", SystemName = SystemPermissionNames.ManageSMSProviders, Category = "Configuration" };
        //public static readonly PermissionRecord ManageGeoIPProviders = new PermissionRecord { Name = "Manage Geo IP Providers", SystemName = SystemPermissionNames.ManageGeoIPProviders, Category = "Configuration" };
        
        //
        //public static readonly PermissionRecord ManagePlugins = new PermissionRecord { Name = "Manage Plugins", SystemName = SystemPermissionNames.ManagePlugins, Category = "Configuration" };
        
        
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[] 
            {
                AccessDashboard,
                
                ViewCompanies, ManageCompanies, 
                ViewBranchOffices, ManageBranchOffices,
                ViewClients, ManageClients,
                ManageUsers, ManageUsersRoles, 
                ManageSystemLog, ManageMessageQueue, ManageMaintenance,ManageMaintenance, ManageMessageTemplates,ManageEmailAccounts, ManageCountries
            };
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[] 
            {
                new DefaultPermissionRecord 
                {
                    UserRoleSystemName = SystemUserRoleNames.Administrators,
                    PermissionRecords = new[] 
                    {
                        AccessDashboard,
                        ViewBranchOffices, ManageBranchOffices,
                        ViewCompanies, ManageCompanies, 
                        ViewClients, ManageClients, 
                        ManageUsers, ManageUsersRoles,
                        ManageSystemLog, ManageMessageQueue, ManageMaintenance, 
                        ManageMessageTemplates, ManageEmailAccounts, ManageCountries
                    }
                },
                new DefaultPermissionRecord 
                {
                    UserRoleSystemName = SystemUserRoleNames.Users,
                    PermissionRecords = new[] 
                    {
                        AccessDashboard,
                        ViewClients,
                        ViewBranchOffices,
                        ViewCompanies
                    }
                },
            };
        }
    }
}