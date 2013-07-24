using System;
using System.Linq;
using System.Xml;
using eCentral.Core;
using eCentral.Core.Domain.Localization;
using eCentral.Services.Localization;
//using OfficeOpenXml;

namespace eCentral.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        private readonly ILanguageService languageService;
        private readonly ILocalizationService localizationService;
        
        public ImportManager(ILanguageService languageService,
            ILocalizationService localizationService)
        {
            this.languageService     = languageService;
            this.localizationService = localizationService;
        }

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        /// <summary>
        /// Import language resources from XML file
        /// </summary>
        /// <param name="language">Language</param>
        /// <param name="xml">XML</param>
        public virtual void ImportLanguageFromXml(Language language, string xml)
        {
            Guard.IsNotNull(language, "Language");

            if (String.IsNullOrEmpty(xml))
                return;

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            var nodes = xmlDoc.SelectNodes(@"//Language/LocaleResource");
            foreach (XmlNode node in nodes)
            {
                string name = node.Attributes["Name"].InnerText.Trim();
                string value = "";
                var valueNode = node.SelectSingleNode("Value");
                if (valueNode != null)
                    value = valueNode.InnerText;
                
                if (String.IsNullOrEmpty(name))
                    continue;
                
                //do not use localizationservice because it'll clear cache and after adding each resoruce
                //let's bulk insert
                var resource = language.LocaleStringResources.Where(x => x.ResourceName.Equals(name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (resource != null)
                {
                    resource.ResourceValue = value;
                    resource.UpdatedOn = DateTime.UtcNow;
                }
                else
                {
                    language.LocaleStringResources.Add(
                        new LocaleStringResource()
                        {
                            ResourceName = name,
                            ResourceValue = value, 
                            CreatedOn = DateTime.UtcNow, 
                            UpdatedOn = DateTime.UtcNow
                        });
                }
            }
            languageService.Update(language);

            //clear cache
            localizationService.ClearCache();
        }
    }
}
