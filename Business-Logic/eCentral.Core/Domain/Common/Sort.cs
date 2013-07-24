using System.ComponentModel;

namespace eCentral.Core.Domain.Common
{
    /// <summary>
    /// Represents the sort 
    /// </summary>
    public class Sort
    {
        /// <summary>
        /// Gets or set the object property name that the data source needs to be sorted on
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or set the sort direction for the property 
        /// </summary>
        public ListSortDirection SortDirection { get; set; }
    }
}