using eCentral.Core.Domain.Localization;

namespace eCentral.Services.ExportImport
{
    /// <summary>
    /// Import manager interface
    /// </summary>
    public interface IImportManager
    {
        /// <summary>
        /// Import language resources from XML file
        /// </summary>
        /// <param name="language">Language</param>
        /// <param name="xml">XML</param>
        void ImportLanguageFromXml(Language language, string xml);
    }
}
