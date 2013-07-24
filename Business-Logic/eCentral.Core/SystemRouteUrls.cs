namespace eCentral.Core
{
    /// <summary>
    /// Represents all the urls used in the application for the routenames
    /// </summary>
    public static partial class SystemRouteUrls
    {
        #region Application 
        
        //login 
        public static string Login { get { return "login/"; } }
        public static string Logout { get { return "logout/"; } }

        // password recovery & security
        public static string ChangePassword { get { return "security/change-password"; } }
        public static string PasswordRecovery { get { return "password-recovery"; } }

        // clients
        public static string Client { get { return "admin/clients"; } }
        public static string ClientAdd { get { return "admin/clients/create"; } }
        public static string ClientEdit { get { return "admin/clients/edit/{rowId}"; } }
        public static string ClientChangeStatus { get { return "admin/clients/change-status"; } }
        public static string ClientView { get { return "admin/clients/{rowId}"; } }

        // branch offices
        public static string BranchOffice { get { return "admin/offices"; } }
        public static string BranchOfficeAdd { get { return "admin/offices/create"; } }
        public static string BranchOfficeEdit { get { return "admin/offices/edit/{rowId}"; } }
        public static string BranchOfficeChangeStatus { get { return "admin/offices/change-status"; } }
        public static string BranchOfficeView { get { return "admin/offices/{rowId}"; } }

        // company
        public static string Company { get { return "admin/company"; } }
        public static string CompanyAdd { get { return "admin/company/create"; } }
        public static string CompanyEdit { get { return "admin/company/edit/{rowId}"; } }
        public static string CompanyView { get { return "admin/company/{rowId}"; } }

        // users
        public static string User { get { return "admin/users"; } }
        public static string UserAdd { get { return "admin/users/create"; } }
        public static string UserEdit { get { return "admin/users/edit/{rowId}"; } }
        public static string UserChangeStatus { get { return "admin/users/change-status"; } }
        public static string UserProfile { get { return "profile/{rowId}"; } }

        // configuration
        //public static string Setting { get { return "admin/settings"; } }
        //public static string Country { get { return "Country"; } }
        //public static string CountryAdd { get { return "CountryAdd"; } }
        //public static string Language { get { return "Language"; } }
        //public static string EmailAccount { get { return "EmailAccount"; } }
        //public static string ActivityLog { get { return "ActivityLog"; } }
        //public static string Plugin { get { return "Plugin"; } }

        // system 
        //public static string SystemLog { get { return "SystemLog"; } }
        //public static string QueuedEmail { get { return "QueuedEmail"; } }
        //public static string SystemInfo { get { return "system/info"; } }

        // miscellaneous
        public static string AsyncUpload { get { return "media/asyncupload"; } }
        public static string Search { get { return "search"; } }

        //some AJAX links
        public static string GetStatesByCountry { get { return "country/statesbycountry"; } }

        // static
        public static string SiteClosed { get { return "site-closed"; } }
        public static string PageNotFound { get { return "page-not-found"; } }
        public static string Error { get { return "server-error"; } }
        public static string AccessDenied { get { return "access-denied"; } }

        #endregion
    }
}