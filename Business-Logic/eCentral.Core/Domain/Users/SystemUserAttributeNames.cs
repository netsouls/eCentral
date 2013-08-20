
namespace eCentral.Core.Domain.Users
{
    public static partial class SystemUserAttributeNames
    {
        //Form fields
        public static string FirstName { get { return "FirstName"; } }

        public static string LastName { get { return "LastName"; } }

        public static string Mobile { get { return "MobileNumber"; }}

        public static string AccountActivationToken { get { return "AccountActivationToken"; } }

        public static string PasswordRecoveryToken { get { return "PasswordRecoveryToken"; } }

        public static string ImpersonatedUserId { get { return "ImpersonatedUserId"; } }

        public static string AssociatedBrancOffices { get { return "AssociatedBrancOffices"; } }
    }
}