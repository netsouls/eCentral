using System.Collections.Generic;
using eCentral.Web.Framework.Mvc;

namespace eCentral.Web.Models.Common
{
    public class SiteThemeSelectorModel : BaseModel
    {
        public SiteThemeSelectorModel()
        {
            AvailableThemes = new List<SiteThemeModel>();
        }

        public IList<SiteThemeModel> AvailableThemes { get; set; }

        public SiteThemeModel CurrentTheme { get; set; }
    }
}