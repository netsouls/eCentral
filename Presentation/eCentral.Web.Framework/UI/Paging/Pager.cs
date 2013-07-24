using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using eCentral.Core.Infrastructure;
using eCentral.Services.Localization;

namespace eCentral.Web.Framework.UI.Paging
{
	/// <summary>
    /// Renders a pager component from an IPageableModel datasource.
	/// </summary>
	public partial class Pager : IHtmlString
	{
        protected readonly IPageableModel model;
        protected readonly ViewContext viewContext;
        protected string pageQueryName = "pagenumber";
        protected string alignment = "centered";
        protected bool isHashTag = false;

        protected bool showTotalSummary;
        protected bool showPagerItems = true;
        protected bool showFirst = true;
        protected bool showPrevious = true;
        protected bool showNext = true;
        protected bool showLast = true;
        protected bool showIndividualPages = true;
        protected int individualPagesDisplayedCount = 5;
        protected IList<string> ignoreParameter;

        protected Func<int, string> urlBuilder;
        protected IList<string> booleanParameterNames;

		public Pager(IPageableModel model, ViewContext context)
		{
            this.model                 = model;
            this.viewContext           = context;
            this.urlBuilder            = CreateDefaultUrl;
            this.booleanParameterNames = new List<string>();
            this.ignoreParameter = new List<string> { "_" };
		}

		protected ViewContext ViewContext 
		{
			get { return viewContext; }
		}
        
        public Pager QueryParam(string value)
		{
            this.pageQueryName = value;
			return this;
		}
        public Pager ShowTotalSummary(bool value)
        {
            this.showTotalSummary = value;
            return this;
        }
        public Pager ShowPagerItems(bool value)
        {
            this.showPagerItems = value;
            return this;
        }
        public Pager Alignment ( string value)
        {
            this.alignment = value;
            return this;
        }

        public Pager IsHashTag( bool value )
        {
            this.isHashTag = value;
            return this;
        }

        public Pager ShowFirst(bool value)
        {
            this.showFirst = value;
            return this;
        }
        public Pager ShowPrevious(bool value)
        {
            this.showPrevious = value;
            return this;
        }
        public Pager ShowNext(bool value)
        {
            this.showNext = value;
            return this;
        }
        public Pager ShowLast(bool value)
        {
            this.showLast = value;
            return this;
        }
        public Pager ShowIndividualPages(bool value)
        {
            this.showIndividualPages = value;
            return this;
        }
        public Pager IndividualPagesDisplayedCount(int value)
        {
            this.individualPagesDisplayedCount = value;
            return this;
        }
		public Pager Link(Func<int, string> value)
		{
            this.urlBuilder = value;
			return this;
		}
        //little hack here due to ugly MVC implementation
        //find more info here: http://www.mindstorminteractive.com/blog/topics/jquery-fix-asp-net-mvc-checkbox-truefalse-value/
        public Pager BooleanParameterName(string paramName)
        {
            booleanParameterNames.Add(paramName);
            return this;
        }

        /// <summary>
        /// Add parameters to be ignored in the url 
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public Pager IgnoreParameter(string paramName )
        {
            ignoreParameter.Add(paramName);
            return this;
        }

        public override string ToString()
        {
            return ToHtmlString();
        }

        public string ToHtmlString()
        {
            if (model.TotalItems == 0)
                return null;

            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            var links = new StringBuilder();

            if (showTotalSummary && (model.TotalPages > 0))
            {
                links.Append("<div class=\"summary\">");
                links.Append(string.Format(localizationService.GetResource("Pager.CurrentPage"), model.PageIndex + 1, model.TotalPages, model.TotalItems));
                links.Append("</div>");
            }

            if (showPagerItems && (model.TotalPages > 1))
            {
                links.Append("<ul>");

                if (showFirst)
                {
                    if ((model.PageIndex >= 3) && (model.TotalPages > individualPagesDisplayedCount))
                    {
                        links.Append(CreatePageLink(1, localizationService.GetResource("Pager.First")));

                        if ((showIndividualPages || (showPrevious && (model.PageIndex > 0))) || showLast)
                        {
                            links.Append("<li class=\"text\">...</li>");
                        }
                    }
                }

                if (showPrevious)
                {
                    links.Append(CreatePageLink(model.PageIndex, localizationService.GetResource("Pager.Previous"), false, !(model.PageIndex >0)));
                }

                if (showIndividualPages)
                {
                    int firstIndividualPageIndex = GetFirstIndividualPageIndex();
                    int lastIndividualPageIndex = GetLastIndividualPageIndex();

                    for (int iKey = firstIndividualPageIndex; iKey <= lastIndividualPageIndex; iKey++)
                    {
                        links.Append(CreatePageLink(iKey + 1, (iKey + 1).ToString(), model.PageIndex == iKey));    
                    }
                }

                if (showNext)
                {
                    links.Append(CreatePageLink(model.PageIndex + 2, localizationService.GetResource("Pager.Next"), false, !((model.PageIndex + 1) < model.TotalPages)));
                }

                if (showLast)
                {
                    if (((model.PageIndex + 3) < model.TotalPages) && (model.TotalPages > individualPagesDisplayedCount))
                    {
                        if (showIndividualPages || (showNext && ((model.PageIndex + 1) < model.TotalPages)))
                        {
                            links.Append("<li class=\"text\">...</li>");
                        }

                        links.Append(CreatePageLink(model.TotalPages, localizationService.GetResource("Pager.Last")));
                    }
                }

                links.Append("</ul>");
            }

            // create the div for the pagination to reside
            StringBuilder pagination = new StringBuilder();
            pagination.AppendFormat("<div class=\"pagination pagination-{0}\">", alignment);
            pagination.Append(links.ToString());
            pagination.Append("</div>");
            
            return pagination.ToString();
        }

        protected virtual int GetFirstIndividualPageIndex()
        {
            if ((model.TotalPages < individualPagesDisplayedCount) ||
                ((model.PageIndex - (individualPagesDisplayedCount / 2)) < 0))
            {
                return 0;
            }
            if ((model.PageIndex + (individualPagesDisplayedCount / 2)) >= model.TotalPages)
            {
                return (model.TotalPages - individualPagesDisplayedCount);
            }
            return (model.PageIndex - (individualPagesDisplayedCount / 2));
        }

        protected virtual int GetLastIndividualPageIndex()
        {
            int num = individualPagesDisplayedCount / 2;
            if ((individualPagesDisplayedCount % 2) == 0)
            {
                num--;
            }
            if ((model.TotalPages < individualPagesDisplayedCount) ||
                ((model.PageIndex + num) >= model.TotalPages))
            {
                return (model.TotalPages - 1);
            }
            if ((model.PageIndex - (individualPagesDisplayedCount / 2)) < 0)
            {
                return (individualPagesDisplayedCount - 1);
            }
            return (model.PageIndex + num);
        }
		protected virtual string CreatePageLink(int pageNumber, string text, bool isCurrentPage = false, bool isDisabled = false)
		{
            var liBulder = new TagBuilder("li");
            if (isDisabled)
                liBulder.AddCssClass("disabled");
            else if (isCurrentPage)
                liBulder.AddCssClass("active");

            if (text.Equals("..."))
                liBulder.AddCssClass("text");

			var builder = new TagBuilder("a");
			builder.SetInnerText(text);
            var actionLink = urlBuilder(pageNumber);

            builder.MergeAttribute("href", string.Format("{0}{1}", isHashTag ? "#" : string.Empty , actionLink));

            liBulder.InnerHtml = builder.ToString(TagRenderMode.Normal);
			return liBulder.ToString(TagRenderMode.Normal);
		}

        protected virtual string CreateDefaultUrl(int pageNumber)
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

			routeValues[pageQueryName] = pageNumber;

			var url = UrlHelper.GenerateUrl(null, null, null, routeValues, RouteTable.Routes, viewContext.RequestContext, true);
			return url;
		}
	}
}