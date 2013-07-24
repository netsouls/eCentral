//Contributor : MVCContrib

using System.Collections;
using System.Collections.Generic;

namespace eCentral.Web.Framework.UI.Sorting
{
    /// <summary>
    /// A collection of objects for sorring.
    /// </summary>
    public interface ISortableModel 
    {
        /// <summary>
        /// Get whether sorting is allowed or not
        /// </summary>
        bool AllowSorting { get; }

        /// <summary>
        /// The current sort order (default is 10 Name Asscending)
        /// </summary>
        int OrderBy{ get; }
    }
}