
namespace eCentral.Services.Cms
{
    /// <summary>
    /// Content service interface
    /// </summary>
    public partial interface IContentService
    {
        /// <summary>
        /// Load content provider by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Html Content</returns>
        string LoadBySystemName(string systemName, string languageCulture);
    }
}
