using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace eCentral.Web.Framework
{
    /// <summary>
    /// Parses the request values from a query from the DataTables jQuery pluggin
    /// </summary>
    /// <typeparam name="T">List data type</typeparam>
    public class DataTablesParser<T> where T : class
    {
       /*
        * int: iDisplayStart - Display start point
        * int: iDisplayLength - Number of records to display
        * string: string: sSearch - Global search field
        * boolean: bEscapeRegex - Global search is regex or not
        * int: iColumns - Number of columns being displayed (useful for getting individual column search info)
        * string: sSortable_(int) - Indicator for if a column is flagged as sortable or not on the client-side
        * string: sSearchable_(int) - Indicator for if a column is flagged as searchable or not on the client-side
        * string: sSearch_(int) - Individual column filter
        * boolean: bEscapeRegex_(int) - Individual column filter is regex or not
        * int: iSortingCols - Number of columns to sort on
        * int: iSortCol_(int) - Column being sorted on (you will need to decode this number for your database)
        * string: sSortDir_(int) - Direction to be sorted - "desc" or "asc". Note that the prefix for this variable is wrong in 1.5.x, but left for backward compatibility)
        * string: sEcho - Information for DataTables to use for rendering
        */

        private const string INDIVIDUAL_SEARCH_KEY_PREFIX = "sSearch_";
        private const string INDIVIDUAL_SORT_KEY_PREFIX = "iSortCol_";
        private const string INDIVIDUAL_SORT_DIRECTION_KEY_PREFIX = "sSortDir_";
        private const string DISPLAY_START = "iDisplayStart";
        private const string DISPLAY_LENGTH = "iDisplayLength";
        private const string ECHO = "sEcho";
        private const string ASCENDING_SORT = "asc";

        private IQueryable<T> _queriable;
        private readonly HttpRequestBase _httpRequest;
        private readonly Type _type;
        private PropertyInfo[] _properties;

        #region Ctor

        public DataTablesParser(HttpRequestBase httpRequest, IQueryable<T> queriable)
        {
            _queriable = queriable;
            _httpRequest = httpRequest;
            _type = typeof(T);
            _properties = _type.GetProperties();
        }

        public DataTablesParser(HttpRequest httpRequest, IQueryable<T> queriable)
            : this(new HttpRequestWrapper(httpRequest), queriable)
        { }

        public DataTablesParser(HttpRequestBase httpRequest, IEnumerable<T> enumerable)
            : this(httpRequest, enumerable.AsQueryable())
        { }

        #endregion

        /// <summary>
        /// Parses the <see cref="HttpRequestBase"/> parameter values for the accepted 
        /// DataTable request values
        /// </summary>
        /// <returns>Formated output for DataTables, which should be serialized to JSON</returns>
        public FormatedList<T> Parse()
        {
            var list = new FormatedList<T>();

            // parse the echo property (must be returned as int to prevent XSS-attack)
            list.sEcho = int.Parse(_httpRequest[ECHO]);

            // count the record BEFORE filtering
            list.iTotalRecords = _queriable.Count();

            // apply the sort, if there is one
            ApplySort();

            // parse the paging values
            int skip = 0, take = 10;
            int.TryParse(_httpRequest[DISPLAY_START], out skip);
            int.TryParse(_httpRequest[DISPLAY_LENGTH], out take);

            //This needs to be an expression or else it won't limit results
            Func<T, bool> GenericFind = delegate(T item)
            {
                bool bFound = false;
                var sSearch = _httpRequest["sSearch"];

                if (string.IsNullOrWhiteSpace(sSearch))
                {
                    return true;
                }

                foreach (PropertyInfo property in _properties)
                {
                    if (Convert.ToString(property.GetValue(item, null)).ToLower().Contains((sSearch).ToLower()))
                    {
                        bFound = true;
                    }
                }
                return bFound;

            };

            // setup the data with individual property search, all fields search,
            // paging, and property list selection
            var resultQuery = _queriable.Where(GenericFind)
                        .Skip(skip)
                        .Take(take);

            list.aaData = resultQuery
                        .ToList();

            list.SetQuery(resultQuery.ToString());

            // total records that are displayed after filter
            list.iTotalDisplayRecords = _queriable.Count(GenericFind);
            return list;
        }

        #region Utilites

        private void ApplySort()
        {
            var thenBy = false;
            var paramExpr = Expression.Parameter(typeof(T), "val");

            // enumerate the keys for any sortations
            foreach (string key in _httpRequest.Params.AllKeys.Where(x => x.StartsWith(INDIVIDUAL_SORT_KEY_PREFIX)))
            {
                // column number to sort (same as the array)
                int sortcolumn = int.Parse(_httpRequest[key]);

                // ignore malformatted values
                if (sortcolumn < 0 || sortcolumn >= _properties.Length)
                    break;

                // get the direction of the sort
                string sortdir = _httpRequest[INDIVIDUAL_SORT_DIRECTION_KEY_PREFIX + key.Replace(INDIVIDUAL_SORT_KEY_PREFIX, string.Empty)];

                // form the sortation per property via a property expression

                //var expression = Expression.Convert(Expression.Property(paramExpr, _properties[sortcolumn].Name),typeof(object));
                var expression1 = Expression.Property(paramExpr, _properties[sortcolumn].Name);
                var propType = _properties[sortcolumn].PropertyType;
                var delegateType = Expression.GetFuncType(typeof(T), propType);
                var propertyExpr = Expression.Lambda(delegateType, expression1, paramExpr);

                // apply the sort (default is ascending if not specified)
                if (string.IsNullOrEmpty(sortdir) || sortdir.Equals(ASCENDING_SORT, StringComparison.OrdinalIgnoreCase))
                    //_queriable = _queriable.OrderBy<T, dynamic>(propertyExpr);

                    _queriable = typeof(Queryable).GetMethods().Single(
                        method => method.Name == (thenBy ? "ThenBy" : "OrderBy")
                                    && method.IsGenericMethodDefinition
                                    && method.GetGenericArguments().Length == 2
                                    && method.GetParameters().Length == 2)
                            .MakeGenericMethod(typeof(T), propType)
                            .Invoke(null, new object[] { _queriable, propertyExpr }) as IOrderedQueryable<T>;

                else
                    //_queriable = _queriable.OrderByDescending<T, dynamic>(propertyExpr);

                    _queriable = typeof(Queryable).GetMethods().Single(
            method => method.Name == (thenBy ? "ThenByDescending" : "OrderByDescending")
                    && method.IsGenericMethodDefinition
                    && method.GetGenericArguments().Length == 2
                    && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), propType)
            .Invoke(null, new object[] { _queriable, propertyExpr }) as IOrderedQueryable<T>;

                thenBy = true;
            }

            //Linq to entities needs a sort to implement skip
            if (!thenBy && !(_queriable is IOrderedQueryable<T>))
            {
                var firstProp = Expression.Property(paramExpr, _properties[0].Name);
                var propType = _properties[0].PropertyType;
                var delegateType = Expression.GetFuncType(typeof(T), propType);
                var propertyExpr = Expression.Lambda(delegateType, firstProp, paramExpr);

                _queriable = typeof(Queryable).GetMethods().Single(
                    method => method.Name == "OrderBy"
                         && method.IsGenericMethodDefinition
                         && method.GetGenericArguments().Length == 2
                         && method.GetParameters().Length == 2)
                 .MakeGenericMethod(typeof(T), propType)
                 .Invoke(null, new object[] { _queriable, propertyExpr }) as IOrderedQueryable<T>;
            }
        }

        #endregion
    }

    public class FormatedList<T>
    {
        private string _query;

        internal void SetQuery(string query)
        {
            _query = query;
        }

        public string GetQuery()
        {
            return _query;
        }

        public int sEcho { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public List<T> aaData { get; set; }
    }
}
