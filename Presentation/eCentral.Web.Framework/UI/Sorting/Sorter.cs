using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using eCentral.Core.Domain.Common;
using eCentral.Core.Infrastructure;
using eCentral.Services.Localization;

namespace eCentral.Web.Framework.UI.Sorting
{
    /// <summary>
    /// Renders a sorting component.
    /// </summary>
    public partial class Sorter : IHtmlString
    {
        protected readonly ISortableModel model;
        protected readonly ViewContext viewContext;
        protected string sortQueryName = "orderby";
        protected bool isHashTag = false;

        protected Func<int, string> urlBuilder;
        protected IList<string> booleanParameterNames;
        protected IList<string> ignoreParameter;
        protected IList<SortingEnum> ignoreSortFor;

        public Sorter(ISortableModel model, ViewContext context)
        {
            this.model = model;
            this.viewContext = context;
            this.urlBuilder = CreateDefaultUrl;
            this.booleanParameterNames = new List<string>();
            this.ignoreParameter = new List<string> { "_" };
            this.ignoreSortFor = new List<SortingEnum>();
        }

        protected ViewContext ViewContext
        {
            get { return viewContext; }
        }

        public Sorter QueryParam(string value)
        {
            this.sortQueryName = value;
            return this;
        }

        public Sorter IsHashTag(bool value)
        {
            this.isHashTag = value;
            return this;
        }

        public Sorter Link(Func<int, string> value)
        {
            this.urlBuilder = value;
            return this;
        }

        //little hack here due to ugly MVC implementation
        //find more info here: http://www.mindstorminteractive.com/blog/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
        public Sorter BooleanParameterName(string paramName)
        {
            booleanParameterNames.Add(paramName);
            return this;
        }

        /// <summary>
        /// Add parameters to be ignored in the url 
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public Sorter IgnoreParameter(string paramName)
        {
            ignoreParameter.Add(paramName);
            return this;
        }

        /// <summary>
        /// Add parammeters to be ignored for sorting
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public Sorter IgnoreSortFor(SortingEnum paramValue)
        {
            ignoreSortFor.Add(paramValue);
            return this;
        }

        public override string ToString()
        {
            return ToHtmlString();
        }

        public string ToHtmlString()
        {
            StringBuilder sorting = new StringBuilder();

            if (model.AllowSorting)
            {
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                var links = new StringBuilder();

                links.Append("<ul>");

                var enumValues = Enum.GetValues(typeof(SortingEnum))
                    .Cast<SortingEnum>()
                    .Where(s => !ignoreSortFor.Contains(s));

                foreach (SortingEnum enumValue in enumValues)
                {
                    links.Append(CreateSortLink(enumValue, localizationService.GetResource("Sorter." + enumValue.ToString() ), model.OrderBy.Equals((int)enumValue)));
                }

                // create the div for the pagination to reside
                sorting.Append("<div class=\"sorting\">");
                sorting.Append(links.ToString());
                sorting.Append("</div>");
            }

            // need clear whether we are doing sorting or not
            sorting.Append("<div class=\"clear\"></div>");

            return sorting.ToString();
        }

        protected virtual string CreateSortLink(SortingEnum orderBy, string title, bool isCurrentPage = false)
        {
            var liBulder = new TagBuilder("li");

            liBulder.Attributes.Add("id", orderBy.ToString().ToLower());

            if (isCurrentPage)
                liBulder.AddCssClass("active");

            var builder = new TagBuilder("a");
            builder.SetInnerText(orderBy.ToString());
            var actionLink = urlBuilder((int)orderBy);

            builder.MergeAttribute("href", string.Format("{0}{1}", isHashTag ? "#" : string.Empty, actionLink));
            builder.MergeAttribute("title", title);

            liBulder.InnerHtml = builder.ToString(TagRenderMode.Normal);
            return liBulder.ToString(TagRenderMode.Normal);
        }

        protected virtual string CreateDefaultUrl(int order)
        {
            var routeValues = new RouteValueDictionary();

            var queryStringKeys = viewContext.RequestContext.HttpContext.Request.QueryString.AllKeys
                .Where(key => key != null && !ignoreParameter.Contains(key));

            foreach (var key in queryStringKeys)
            {
                var value = viewContext.RequestContext.HttpContext.Request.QueryString[key];
                if (booleanParameterNames.Contains(key, StringComparer.InvariantCultureIgnoreCase))
                {
                    //little hack here due to ugly MVC implementation
                    //find more info here: http://www.mindstorminteractive.com/blog/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
                    if (!String.IsNullOrEmpty(value) && value.Equals("true,false", StringComparison.InvariantCultureIgnoreCase))
                    {
                        value = "true";
                    }
                }
                routeValues[key] = value;
            }

            routeValues[sortQueryName] = order;

            var url = UrlHelper.GenerateUrl(null, null, null, routeValues, RouteTable.Routes, viewContext.RequestContext, true);
            return url;
        }
    }
}