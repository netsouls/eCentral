namespace eCentral.Core.Domain.Common
{
    /// <summary>
    /// Represents the website sorting
    /// </summary>
    public enum SortingEnum
    {
        /// <summary>
        /// Name: A to Z
        /// </summary>
        NameAsc = 10,

        /// <summary>
        /// Name: Z to A
        /// </summary>
        NameDesc = 11,

        /// <summary>
        /// creation date
        /// </summary>
        CreatedOn = 12,

        /// <summary>
        /// updated on
        /// </summary>
        UpdatedOn = 13
    }
}