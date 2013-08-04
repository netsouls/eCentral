using System;
using System.Web.Mvc;
using eCentral.Web.Framework.Mvc;

namespace eCentral.Web.Models.Logging
{
    public partial class LogModel : BaseEntityModel
    {
        public string LogLevel { get; set; }

        [AllowHtml]
        public string ShortMessage { get; set; }

        [AllowHtml]
        public string FullMessage { get; set; }

        [AllowHtml]
        public string IpAddress { get; set; }

        [AllowHtml]
        public string PageUrl { get; set; }

        [AllowHtml]
        public string ReferrerUrl { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid? UserId { get; set; }
        public string UserName { get; set; }
    }
}