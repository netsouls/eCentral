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
        //public static string ChangePassword { get { return "security/change-password"; } }
        //public static string PasswordRecovery { get { return "password-recovery"; } }

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