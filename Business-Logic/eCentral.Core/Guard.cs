using System;
using System.Collections.Generic;

namespace eCentral.Core
{
    /// <summary>
    /// Guard against incorrect values
    /// </summary>
    public static class Guard
    {
        private static readonly string CannotBeNegative = "\"{0}\" cannot be negative.";
        private static readonly string CannotBeNull = "\"{0}\" cannot be null.";
        private static readonly string CannotBeNullOrEmpty = "\"{0}\" cannot be null or empty.";
        private static readonly string ArrayCannotBeEmpty = "\"{0}\" array cannot be empty.";
        private static readonly string CollectionCannotBeEmpty = "\"{0}\" collection cannot be empty.";
        private static readonly string SourceMustBeAVirtualPathWhichShouldStartsWithTileAndSlash = "Source must be a virtual path which should starts with \"~/\"";
        private static readonly string CannotBeNegativeOrZero = "\"{0}\" cannot be negative or zero.";

        public static void IsNotNegative(int parameter, string parameterName)
        {
            if (parameter < 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, CannotBeNegative.FormatWith(new object[] { parameterName}));
            }
        }

        public static void IsNotNegative(float parameter, string parameterName)
        {
            if (parameter < 0f)
            {
                throw new ArgumentOutOfRangeException(parameterName, CannotBeNegative.FormatWith(new object[] { parameterName }));
            }
        }

        public static void IsNotEmpty(Guid parameter, string parameterName)
        {
            if (parameter.Equals(Guid.Empty))
            {
                throw new ArgumentNullException(parameterName, CannotBeNull.FormatWith(new object[] { parameterName }));
            }
        }

        public static void IsNotNull(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName, CannotBeNull.FormatWith(new object[] { parameterName }));
            }
        }

        public static void IsNotNullOrEmpty<T>(T[] parameter, string parameterName)
        {
            IsNotNull(parameter, parameterName);
            if (parameter.Length == 0)
            {
                throw new ArgumentException(ArrayCannotBeEmpty.FormatWith(new object[] { parameterName }));
            }
        }

        public static void IsNotNullOrEmpty<T>(ICollection<T> parameter, string parameterName)
        {
            IsNotNull(parameter, parameterName);
            if (parameter.Count == 0)
            {
                throw new ArgumentException(CollectionCannotBeEmpty.FormatWith(new object[] { parameterName }), parameterName);
            }
        }

        public static void IsNotNullOrEmpty(string parameter, string parameterName)
        {
            if (string.IsNullOrEmpty(parameter ?? string.Empty))
            {
                throw new ArgumentException(CannotBeNullOrEmpty.FormatWith(new object[] { parameterName }));
            }
        }

        public static void IsNotVirtualPath(string parameter, string parameterName)
        {
            IsNotNullOrEmpty(parameter, parameterName);
            if (!parameter.StartsWith("~/", StringComparison.Ordinal))
            {
                throw new ArgumentException(SourceMustBeAVirtualPathWhichShouldStartsWithTileAndSlash, parameterName);
            }
        }

        public static void IsNotZeroOrNegative(int parameter, string parameterName)
        {
            if (parameter <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, CannotBeNegativeOrZero.FormatWith(new object[] { parameterName }));
            }
        }

    }
}
