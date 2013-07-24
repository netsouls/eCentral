using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using eCentral.Core;
using eCentral.Core.Domain;
using eCentral.Core.Domain.Localization;

namespace eCentral.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        private readonly SiteInformationSettings siteInformationSettings;
        
        public ExportManager(SiteInformationSettings siteInformationSettings)
        {
            this.siteInformationSettings = siteInformationSettings;
        }

        #region Utilities

        #endregion

        #region Methods

        /// <summary>
        /// Export language resources to xml
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>Result in XML format</returns>
        public virtual string ExportLanguageToXml(Language language)
        {
            Guard.IsNotNull(language, "Language");

            var sb           = new StringBuilder();
            var stringWriter = new StringWriter(sb);
            var xmlWriter    = new XmlTextWriter(stringWriter);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Language");
            xmlWriter.WriteAttributeString("Name", language.Name);

            var resources = language.LocaleStringResources.OrderBy(x => x.ResourceName).ToList();
            foreach (var resource in resources)
            {
                xmlWriter.WriteStartElement("LocaleResource");
                xmlWriter.WriteAttributeString("Name", resource.ResourceName);
                xmlWriter.WriteAttributeString("IsJsonResource", resource.IsJsonResource.ToString());
                xmlWriter.WriteElementString("Value", null, resource.ResourceValue);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
            return stringWriter.ToString();
        }

        #endregion
    }
}
