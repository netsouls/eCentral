using System.Collections.Generic;
using eCentral.Web.Framework.Mvc;

namespace eCentral.Web.Models.Common
{
    public class LanguageSelectorModel : BaseModel
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public LanguageModel CurrentLanguage { get; set; }

        public bool UseImages { get; set; }
    }
}