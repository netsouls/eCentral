using System;

namespace eCentral.Web.Models.Common
{
    public partial class ChangeStatusModel
    {
        /// <summary>
        /// Status that needs to be changed to
        /// </summary>
        public int StatusId { get; set;}

        public Guid[] RowIds { get; set; }
    }
}
