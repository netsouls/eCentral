namespace eCentral.Core
{
    /// <summary>
    /// Represents all the route urls in the application
    /// </summary>
    public static partial class SystemRouteNames
    {
        public static string HomePage { get { return "HomePage"; } }

        // standard
        public static string Create { get { return "Create"; } }
        public static string Index { get { return "Index"; } }
        public static string List { get { return "List"; } }
        public static string Edit { get { return "Edit"; } }
        public static string ChangeStatus { get { return "ChangeStatus"; } }
        public static string View { get { return "View"; } }

        //login and user security
        public static string Login { get { return "Login"; } }
        public static string Logout { get { return "Logout"; } }
        public static string AccountActivation { get { return "AccountActivation"; } }
        public static string PasswordRecovery { get { return "PasswordRecovery"; } }
        public static string PasswordRecoveryConfirm { get { return "PasswordRecoveryConfirm"; } }

        // password recovery & security - todo        
        //public static string ChangePassword { get { return "ChangePassword"; } }

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