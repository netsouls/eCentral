using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;

namespace eCentral.Web.Models.Logging
{
    public class LogSearchModel : BaseModel
    {
        public LogSearchModel()
        {
            AvailableLogLevels = new List<SelectListItem>();
        }

        [UIHint("DateNullable")]
        [SiteResourceDisplayName("System.Log.Fields.CreatedOnFrom")]
        public DateTime? CreatedOnFrom { get; set; }

        [UIHint("DateNullable")]
        [SiteResourceDisplayName("System.Log.Fields.CreatedOnTo")]
        public DateTime? CreatedOnTo { get; set; }

        [SiteResourceDisplayName("System.Log.Fields.Message")]
        public string Message{ get; set; }

        [SiteResourceDisplayName("System.Log.Fields.LogLevel")]
        public int LogLevelId { get; set; }

        public IList<SelectListItem> AvailableLogLevels { get; set; }
    }
}