using System.Collections.Generic;

namespace eCentral.Web.Framework.Mvc
{
    public partial class PagedModel<T>
    {
        public IEnumerable<T> Data { get; set; }

        public int TotalCount { get; set; }
    }
}
