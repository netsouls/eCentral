using System;
using System.Collections.Generic;
using System.Linq;

namespace eCentral.Web.Framework
{
    public class TelerikLocalizationService : Telerik.Web.Mvc.Infrastructure.ILocalizationService
    {
        private string _resourceName;
        private eCentral.Services.Localization.ILocalizationService _localizationService;
        private Guid _currentLanguageId;

        public TelerikLocalizationService(string resourceName, Guid currentLanguageId, Services.Localization.ILocalizationService localizationService)
        {
            _resourceName        = resourceName;
            _currentLanguageId   = currentLanguageId;
            _localizationService = localizationService;
        }

        public IDictionary<string, string> All()
        {
            return ScopedResources();
        }

        public bool IsDefault
        {
            get { return true; }
        }

        public string One(string key)
        {
            var resourceName = "Telerik." + _resourceName + "." + key;
            return _localizationService.GetResource(resourceName, _currentLanguageId, true, resourceName);
        }

        private IDictionary<string, string> ScopedResources()
        {
            var scope = "Telerik." + _resourceName;
            return _localizationService.GetAllResourcesByLanguageId(_currentLanguageId).Where(x => x.Key.ToLower().StartsWith(scope)).ToDictionary(x => x.Key.Replace(scope,""),
                                                                                                    x => x.Value.ResourceValue);
        }
    }
}