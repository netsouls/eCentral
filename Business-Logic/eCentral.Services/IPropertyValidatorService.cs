namespace eCentral.Services
{
    /// <summary>
    /// SSI service interface
    /// </summary>
    public partial interface IPropertyValidatorService
    {
        /// <summary>
        /// Check whether the entity is unique
        /// </summary>
        /// <param name="uniqueValue">Unique identity value of the repository</param>
        /// <returns>user</returns>
        bool IsUnique(object uniqueValue);
    }
}