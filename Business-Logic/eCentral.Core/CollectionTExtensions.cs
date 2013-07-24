﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace eCentral.Core
{
    public static class CollectionTExtensions
    {
        public static void AddRange<T>(this ICollection<T> instance, IEnumerable<T> collection)
        {
            Guard.IsNotNull(instance, "instance");
            Guard.IsNotNull(collection, "collection");

            foreach (T local in collection)
                instance.Add(local);
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            Guard.IsNotNull(collection, "collection");

            foreach (T local in collection)
                action(local);
        }

        public static string ToDelimitedString<T>(this IEnumerable<T> collection)
        {
            return collection.ToDelimitedString(",");
        }

        public static string ToDelimitedString<T>(this IEnumerable<T> collection, string delimiter)
        {
            return string.Join(delimiter, collection.Select(x => x.ToString()).ToArray());
        }

        public static ICollection<T> SetupBeforeAndAfterActions<T>(
            this ICollection<T> value,
            Action<dynamic> setParent,
            Action<dynamic> setParentToNull)
            where T : class
        {
            if (!CommonHelper.OneToManyCollectionWrapperEnabled)
                return value;

            var list = value as IPersistentCollection<T> ?? new PersistentCollection<T>(value);
            list.BeforeAdd = (l, x) => l.BeforeAddItem(x, setParent);
            list.BeforeRemove = (l, x) => l.BeforeRemoveItem(x, setParentToNull);
            list.AfterAdd = AfterListChanges;
            list.AfterRemove = AfterListChanges;
            return list;
        }

        public static void AfterListChanges<T>(this ICollection<T> list) where T : class
        {
            // ...
        }

        public static bool BeforeAddItem<T>(this ICollection<T> list, T item, Action<T> setParent) where T : class
        {
            // ...
            setParent(item);
            if (list.Any(item.Equals))
            {
                return false;
            }
            return true;
        }

        public static bool BeforeRemoveItem<T>(this ICollection<T> list, T item, Action<T> setParentToNull) where T : class
        {
            setParentToNull(item);
            if (list.Any(item.Equals))
            {
                return true;
            }
            return false;
        }

        #region Utilities

        #endregion
    }
}
