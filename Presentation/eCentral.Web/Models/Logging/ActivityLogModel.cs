using System;
using eCentral.Web.Framework.Mvc;

namespace eCentral.Web.Models.Logging
{
    public partial class ActivityLogModel : BaseEntityModel
    {
        public string ActivityLogTypeName { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string VersionControl { get; set; }

        public string Comments { get; set; }

        /// <summary>
        /// Get or set the activity createdon
        /// </summary>
        public string CreatedOn{ get; set; }
    }
}