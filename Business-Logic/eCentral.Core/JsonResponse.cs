using System.Collections.Generic;

namespace eCentral.Core
{
    /// <summary>
    /// Respresents a json response result object
    /// </summary>
    public class JsonResponse
    {
        /// <summary>
        /// Gets or set the list of errors
        /// </summary>
        public IList<string> Errors { get; set; }

        #region Ctor

        public JsonResponse()
        {
            this.Errors = new List<string>();
        }

        #endregion

        /// <summary>
        /// Gets or sets whether the action result was success or failure
        /// </summary>
        public bool Success
        {
            get { return (this.Errors.Count == 0); }
        }

        /// <summary>
        /// Add error message
        /// </summary>
        /// <param name="error"></param>
        public void AddError(string error)
        {
            this.Errors.Add(error);
        }

        /// <summary>
        /// Gets a strings of all the errors delimited by | 
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public string ErrorMessages
        {
            get
            {
                return this.Errors.ToDelimitedString("|");
            }
        }

    }

    public class JsonResponse<T> : JsonResponse
    {
        /// <summary>
        /// Gets or set the data item
        /// </summary>
        public T Data { get; set; }
    }
}