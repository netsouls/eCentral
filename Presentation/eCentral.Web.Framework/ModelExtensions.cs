using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using eCentral.Core;

namespace eCentral.Web.Framework
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Get a strings of all the model errors delimited by | 
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static string GetModelErrors (this ModelStateDictionary modelState )
        {
            if (!modelState.IsValid)
            {
                return modelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
                    .ToDelimitedString("|");
            }

            return string.Empty;
        }

        /// <summary>
        /// Get a list of all the model errors 
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static List<string> GetModelErrorsList(this ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                return modelState.Values
                    .SelectMany(m => m.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            }

            return new List<string>();
        }
    }
}

