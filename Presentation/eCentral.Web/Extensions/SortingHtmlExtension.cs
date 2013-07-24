using System.Web.Mvc;
using eCentral.Web.Framework.UI.Sorting;

namespace eCentral.Web.Extensions
{
    public static class SortingHtmlExtension
    {
        public static Sorter Sorting(this HtmlHelper helper, ISortableModel sortable )
        {
            return new Sorter(sortable, helper.ViewContext);
        }
    }
}
