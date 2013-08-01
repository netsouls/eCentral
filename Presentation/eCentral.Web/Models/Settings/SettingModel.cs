using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Settings;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Settings
{
    [Validator(typeof(SettingValidator))]
    public class SettingModel : BaseEntityModel
    {
        [SiteResourceDisplayName("Settings.Advanced.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SiteResourceDisplayName("Settings.Advanced.Fields.Value")]
        [AllowHtml]
        public string Value { get; set; }
    }
}