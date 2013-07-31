using System.Text;
using eCentral.Core.Configuration;

namespace eCentral.Core.Domain.Common
{
    public class SeoSettings : ISettings
    {
        public string PageTitleSeparator { get; set; }
        
        public string DefaultTitle { get; set; }

        #region To String

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            // set values 
            builder.AppendFormat("PageTitleSeparator: [{0}]", this.PageTitleSeparator.Trim())
                .AppendFormat(", DefaultTitle: [{0}]", this.DefaultTitle.Trim());

            return builder.ToString();
        }

        #endregion
    }
}