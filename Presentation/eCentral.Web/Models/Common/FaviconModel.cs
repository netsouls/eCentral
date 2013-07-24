using eCentral.Web.Framework.Mvc;

namespace eCentral.Web.Models.Common
{
    public class FaviconModel : BaseModel
    {
        public bool Uploaded { get; set; }
        public string FaviconUrl { get; set; }
    }
}