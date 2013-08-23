using System;
using eCentral.Core.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eCentral.Core;
using eCentral.Core.Domain.Common;
using eCentral.Web.Framework.HttpHandlers;

namespace eCentral.Web.Framework.UI
{
    public class PageTitleBuilder : IPageTitleBuilder
    {
        private readonly SeoSettings _seoSettings;
        private readonly IWebHelper _webHelper;
        private readonly SiteInformationSettings siteSettings;
        private readonly List<string> _titleParts;
        private readonly List<string> _metaDescriptionParts;
        private readonly List<string> _metaKeywordParts;
        private readonly Dictionary<ResourceLocation, List<string>> _scriptParts;
        private readonly Dictionary<ResourceLocation, List<string>> _cssParts;
        private readonly List<string> _canonicalUrlParts;
                
        public PageTitleBuilder(SeoSettings seoSettings, SiteInformationSettings siteSettings,
            IWebHelper webHelper)
        {
            this._seoSettings          = seoSettings;
            this._webHelper            = webHelper;
            this.siteSettings          = siteSettings;
            this._titleParts           = new List<string>();
            this._metaDescriptionParts = new List<string>();
            this._metaKeywordParts     = new List<string>();
            this._scriptParts          = new Dictionary<ResourceLocation, List<string>>();
            this._cssParts             = new Dictionary<ResourceLocation,List<string>>();
            this._canonicalUrlParts    = new List<string>();
        }

        public void AddTitleParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _titleParts.Add(part);
        }
        public void AppendTitleParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _titleParts.Insert(0, part);
        }
        public string GenerateTitle(bool addDefaultTitle)
        {
            string result = "";
            var specificTitle = string.Join(_seoSettings.PageTitleSeparator, _titleParts.AsEnumerable().Reverse().ToArray());
            if (!String.IsNullOrEmpty(specificTitle))
            {
                if (addDefaultTitle)
                {
                    //page title + site nam
                    result = string.Join(_seoSettings.PageTitleSeparator, specificTitle, _seoSettings.DefaultTitle);
                }
                else
                {
                    //page title only
                    result = specificTitle;
                }
            }
            else
            {
                //store name only
                result = _seoSettings.DefaultTitle;
            }
            return result;
        }


        public void AddScriptParts(ResourceLocation location, params string[] parts)
        {
            if (!_scriptParts.ContainsKey(location))
                _scriptParts.Add(location, new List<string>());

            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _scriptParts[location].Add(part.ToLowerInvariant());
        }

        public void AppendScriptParts(ResourceLocation location, params string[] parts)
        {
            if (!_scriptParts.ContainsKey(location))
                _scriptParts.Add(location, new List<string>());

            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _scriptParts[location].Insert(0, part.ToLowerInvariant());
        }

        public string GenerateScripts(ResourceLocation location)
        {
            if (!_scriptParts.ContainsKey(location) || _scriptParts[location] == null)
                return "";

            // set hashvalue, we set this to the site version  
            var hashValue = string.Format(".js?{0}", SiteVersion.CurrentVersionHashValue);

            var result = new StringBuilder();
            //use only distinct rows
            foreach (var scriptPath in _scriptParts[location].Distinct())
            {
                if (scriptPath.StartsWith("//"))
                    result.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>", scriptPath);
                else
                {
                    if ( siteSettings.ApplicationState == ApplicationState.Development)
                        result.AppendFormat("<script src=\"{0}?{1}={2}\" type=\"text/javascript\"></script>",
                            JsHttpHandler.DefaultPath, JsHttpHandler.IdParameterName, scriptPath.Replace("/", "_").Replace(".js", hashValue));
                    else
                        result.AppendFormat("<script src=\"{0}library/js/{1}\" type=\"text/javascript\"></script>",
                            _webHelper.RelativeWebRoot, scriptPath.Replace(".js", hashValue));
                }   
            }

            return result.ToString();
        }


        public void AddCssFileParts(ResourceLocation location, params string[] parts)
        {
            if (!_cssParts.ContainsKey(location))
                _cssParts.Add(location, new List<string>());

            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _cssParts[location].Add(part);
        }
        public void AppendCssFileParts(ResourceLocation location, params string[] parts)
        {
            if (!_cssParts.ContainsKey(location))
                _cssParts.Add(location, new List<string>());

            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _cssParts[location].Insert(0, part);
        }

        public string GenerateCssFiles(ResourceLocation location)
        {
            if (!_cssParts.ContainsKey(location) || _cssParts[location] == null)
                return "";

            // set hashvalue, we set this to the site version  
            var hashValue = string.Format(".css?{0}", SiteVersion.CurrentVersionHashValue);
            
            var result = new StringBuilder();
            //use only distinct rows
            foreach (var cssPath in _cssParts[location].Distinct())
            {
                if (cssPath.StartsWith("//")) // 30-04-2013: Deepankar - these files are coming from the CDN
                    result.AppendFormat("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", cssPath);
                else
                    result.AppendFormat("<link href=\"{0}?{1}={2}\" rel=\"stylesheet\" type=\"text/css\" />", 
                        CssHttpHandler.DefaultPath, CssHttpHandler.IdParameterName, cssPath.Replace("/","_").Replace(".css", hashValue ));
            }
            return result.ToString();
        }

        public void AddCanonicalUrlParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _canonicalUrlParts.Add(part);
        }
        public void AppendCanonicalUrlParts(params string[] parts)
        {
            if (parts != null)
                foreach (string part in parts)
                    if (!string.IsNullOrEmpty(part))
                        _canonicalUrlParts.Insert(0, part);
        }

        public string GenerateCanonicalUrls()
        {
            var result = new StringBuilder();
            foreach (var canonicalUrl in _canonicalUrlParts)
            {
                result.AppendFormat("<link rel=\"canonical\" href=\"{0}\" />", canonicalUrl);
                //result.Append(Environment.NewLine);
            }
            return result.ToString();
        }
    }
}
