namespace eCentral.Core
{
    /// <summary>
    /// Represents all the objects keys
    /// </summary>
    public static class StateKeyManager
    {
        #region Application keys

        #endregion

        public const string AbsoluteURl = "Site:AbsoluteUrl";

        public const string RelativeWebRoot = "Site:RelativeWebRoot";

        public const string DateFormat = "MMMM dd yyyy";
        public const string DateTimeFormat = "MMM d yyyy HH:mm";

        #region Session Keys 

        #endregion

        #region Security

        public const string EncryptionKey = "zJzpIuF54D"; // IMPORTANT: DO NOT CHANGE THIS VALUE. IF CHANGED THEN ALL AES DECRYPTIONS WOULD FAIL
        public const string TemporaryPassword = "ecentral.tranzlog.com";

        #endregion

        #region RegEx Expressions

        public const string PasswordValidationRegEx           = @"^.*(?=.{6,})(?=.*\d)(?=.*[a-z]).*$";//(?=.*[A-Z])
        public const string EmailValidationRegEx              = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        public const string MobileValidationRegEx             = @"^((\+)?(\d{2}(\-)?))?(\d{10}){1}?$";
        public const string URlValidationRegEx                = @"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
        public const string URlWithoutProtocolValidationRegEx = @"((http|https)://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
        public const string GuidValidationRegEx               = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";
        public const string GuidEmptyValidationRegEx          = @"^(\{{0,1}([0]){8}-([0]){4}-([0]){4}-([0]){4}-([0]){12}\}{0,1})$";

        #endregion
    }
}