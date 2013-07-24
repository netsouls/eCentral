namespace eCentral.Core
{
    /// <summary>
    /// Represents all the route urls in the application
    /// </summary>
    public static partial class SystemRouteNames
    {
        public static string HomePage { get { return "HomePage"; } }

        //login 
        public static string Login { get { return "Login"; } }
        public static string Logout { get { return "Logout"; } }

        // password recovery & security
        public static string PasswordRecovery { get { return "PasswordRecovery"; } }
        public static string ChangePassword { get { return "ChangePassword"; } }

        // clients
        public static string Client { get { return "Client"; } }
        public static string ClientAdd { get { return "ClientAdd"; } }
        public static string ClientEdit { get { return "ClientEdit"; } }
        public static string ClientChangeStatus { get { return "ClientChangeStatus"; } }
        public static string ClientView { get { return "ClientView"; } }

        // branch offices
        public static string BranchOffice { get { return "BranchOffice"; } }
        public static string BranchOfficeAdd { get { return "BranchOfficeAdd"; } }
        public static string BranchOfficeEdit { get { return "BranchOfficeEdit"; } }
        public static string BranchOfficeChangeStatus { get { return "BranchOfficeChangeStatus"; } }
        public static string BranchOfficeView { get { return "BranchOfficeView"; } }

        // company
        public static string Company { get { return "Company"; } }
        public static string CompanyAdd { get { return "CompanyAdd"; } }
        public static string CompanyEdit { get { return "CompanyEdit"; } }
        public static string CompanyView { get { return "CompanyView"; } }

        // users
        public static string User { get { return "User"; } }
        public static string UserAdd { get { return "UserAdd"; } }
        public static string UserEdit { get { return "UserEdit"; } }
        public static string UserChangeStatus { get { return "UserChangeStatus"; } }
        public static string UserProfile { get { return "UserProfile"; } }
        
        // configuration
        //public static string Setting { get { return "Setting"; } }
        //public static string Country { get { return "Country"; } }
        //public static string CountryAdd { get { return "CountryAdd"; } }
        //public static string Language { get { return "Language"; } }
        //public static string EmailAccount { get { return "EmailAccount"; } }
        //public static string ActivityLog { get { return "ActivityLog"; } }
        //public static string Plugin { get { return "Plugin"; } }

        // system 
        //public static string SystemLog { get { return "SystemLog"; } }
        //public static string QueuedEmail { get { return "QueuedEmail"; } }
        //public static string SystemInfo { get { return "SystemInfo"; } }

        // miscellaneous
        public static string AsyncUpload { get { return "AsyncUpload"; } }
        public static string Search { get { return "Search"; } }

        //some AJAX links
        public static string GetStatesByCountry { get { return "GetStatesByCountry"; }}

        // static
        public static string SiteClosed { get { return "SiteClosed"; } }
        public static string PageNotFound { get { return "PageNotFound"; } }
        public static string Error { get { return "Error"; } }
        public static string AccessDenied { get { return "AccessDenied"; } }
    }
}