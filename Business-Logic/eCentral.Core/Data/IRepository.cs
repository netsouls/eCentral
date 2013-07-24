using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace eCentral.Core.Data
{
    /// <summary>
    /// Repository
    /// </summary>
    public partial interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Get the single entity object by primary key
        /// </summary>
        /// <param name="id">row identifier</param>
        /// <returns></returns>
        T GetById(object id);

        /// <summary>
        /// Get all the objects matching the linq expression criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get all the objects matching the linq expression criteria and sorted by the order by clause
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate, string orderBy, ListSortDirection sortOrder);
        
        /// <summary>
        /// Get the single entity object matching the linq expression criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T Single(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get the first entity object matching the linq expression criteria
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T First(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// Add an entity
        /// </summary>
        /// <param name="entity"></param>
        void Insert(T entity);

        /// <summary>
        /// Update an entity
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);

        /// <summary>
        /// Save changes to the context items
        /// </summary>
        void SaveChanges();
 
        /// <summary>
        /// Get all the entity objects
        /// </summary>
        /// <returns></returns>
        IQueryable<T> Table { get; }
    }
}