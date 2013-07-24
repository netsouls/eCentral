using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;

namespace eCentral.Core.Session
{
    /// <summary>
    /// Represents a HttpSessionManager
    /// </summary>
    public partial class SessionManager : ISessionManager
    {
        public HttpSessionState Session
        {
            get
            {
                return HttpContext.Current.Session;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public bool IsSet(string key)
        {
            return (Session.Keys.Cast<string>().Contains(key.ToString()));
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public void Remove(string key)
        {
            Session.Remove(key.ToString());
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();

            foreach (var item in Session.Keys)
                if (regex.IsMatch(item.ToString()))
                    keysToRemove.Add(item.ToString());

            foreach (string key in keysToRemove)
            {
                Remove(key);
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public void Clear()
        {
            Session.Clear();
        }

        /// <summary>
        /// Cancel the current session
        /// </summary>
        public void Abandon()
        {
            Session.Abandon();
        }

        #region Properties 

        /// <summary>
        /// Get an session object by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>
        public virtual T Get<T>(string key, Func<T> acquire)
        {
            if (this.IsSet(key))
            {
                return this.Get<T>(key);
            }
            else
            {
                var result = acquire();
                //if (result != null)
                this.Set(key, result);
                return result;
            }
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        private T Get<T>(string key)
        {
            if ( Session[key] != null )
                return (T)Session[key];

            return default(T);
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        private void Set(string key, object data)
        {
            if (data == null)
                return;

            Session.Add(key.ToString(), data);
        }

        #endregion
    }
}