namespace eCentral.Core.Domain.Users
{
    /// <summary>
    /// Represents the customer registration type fortatting enumeration
    /// </summary>
    public enum UserRegistrationType : int
    {
        /// <summary>
        /// Standard account creation
        /// </summary>
        Standard = 1,

        /// <summary>
        /// Email validation is required after registration
        /// </summary>
        EmailValidation = 2
    }
}
