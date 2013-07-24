using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace eCentral.Web.Framework
{
    public static class GravatarHtmlHelper
    {
        /// <summary>
        /// Creates HTML for an <c>img</c> element that presents a Gravatar icon.
        /// </summary>
        /// <param name="html">The <see cref="HtmlHelper"/> upon which this extension method is provided.</param>
        /// <param name="email">The email address used to identify the icon.</param>
        /// <param name="size">An optional parameter that specifies the size of the square image in pixels.</param>
        /// <param name="rating">An optional parameter that specifies the safety level of allowed images.</param>
        /// <param name="defaultImage">An optional parameter that controls what image is displayed for email addresses that don't have associated Gravatar icons.</param>
        /// <param name="htmlAttributes">An optional parameter holding additional attributes to be included on the <c>img</c> element.</param>
        /// <returns>An HTML string of the <c>img</c> element that presents a Gravatar icon.</returns>
        public static string Gravatar(this HtmlHelper html,
                                      string email,
                                      int? size = null,
                                      GravatarDefaultImage defaultImage = GravatarDefaultImage.Default,
                                      object htmlAttributes = null)
        {
            var url = new StringBuilder("https://www.gravatar.com/avatar/", 90);
            url.Append(GetEmailHash(email));

            var isFirst = true;
            Action<string, string> addParam = (p, v) =>
            {
                url.Append(isFirst ? '?' : '&');
                isFirst = false;
                url.Append(p);
                url.Append('=');
                url.Append(v);
            };

            if (size != null)
            {
                if (size < 1 || size > 512)
                    throw new ArgumentOutOfRangeException("size", size, "Must be null or between 1 and 512, inclusive.");
                addParam("s", size.Value.ToString());
            }

            if (defaultImage != GravatarDefaultImage.Default)
            {
                if (defaultImage == GravatarDefaultImage.Http404)
                    addParam("d", "404");
                else if (defaultImage == GravatarDefaultImage.Identicon)
                    addParam("d", "identicon");
                if (defaultImage == GravatarDefaultImage.MonsterId)
                    addParam("d", "monsterid");
                if (defaultImage == GravatarDefaultImage.MysteryMan)
                    addParam("d", "mm");
                if (defaultImage == GravatarDefaultImage.Wavatar)
                    addParam("d", "wavatar");
            }

            var tag = new TagBuilder("img");
            tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            tag.Attributes.Add("src", url.ToString());

            if (size != null)
            {
                tag.Attributes.Add("width", size.ToString());
                tag.Attributes.Add("height", (size-5).ToString());
            }

            return tag.ToString();
        }

        private static string GetEmailHash(string email)
        {
            if (email == null)
                return new string('0', 32);

            email = email.Trim().ToLower();

            var emailBytes = Encoding.ASCII.GetBytes(email);
            var hashBytes = new MD5CryptoServiceProvider().ComputeHash(emailBytes);

            var hash = new StringBuilder();
            foreach (var b in hashBytes)
                hash.Append(b.ToString("x2"));
            return hash.ToString();
        }
    }

    public enum GravatarDefaultImage
    {
        /// <summary>
        /// The default value image.  That is, the image returned when no specific default value is included
        /// with the request.  At the time of authoring, this image is the Gravatar icon.
        /// </summary>
        Default,

        /// <summary>
        /// Do not load any image if none is associated with the email hash, instead return an HTTP 404 (File Not Found) response.
        /// </summary>
        Http404,

        /// <summary>
        /// A simple, cartoon-style silhouetted outline of a person (does not vary by email hash).
        /// </summary>
        MysteryMan,

        /// <summary>
        /// A geometric pattern based on an email hash.
        /// </summary>
        Identicon,

        /// <summary>
        /// A generated 'monster' with different colors, faces, etc.
        /// </summary>
        MonsterId,

        /// <summary>
        /// Generated faces with differing features and backgrounds.
        /// </summary>
        Wavatar
    }
}
