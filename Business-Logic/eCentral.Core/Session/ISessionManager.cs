using System;
using System.Web.SessionState;

namespace eCentral.Core.Session
{
    /// <summary>
    /// Session manager interface
    /// </summary>
    public interface ISessionManager
    {
        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is in session
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        bool IsSet(string key);

        /// <summary>
        /// Removes the value with the specified key from the session
        /// </summary>
        /// <param name="key">/key</param>
        void Remove(string key);

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// Clear all session data
        /// </summary>
        void Clear();

        /// <summary>
        /// End the user session
        /// </summary>
        void Abandon();

        /// <summary>
        /// Gets the session object
        /// </summary>
        HttpSessionState Session{ get; }

        /// <summary>
        /// Get Session object by Key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T Get<T>(string key, Func<T> acquire);
    }
}
