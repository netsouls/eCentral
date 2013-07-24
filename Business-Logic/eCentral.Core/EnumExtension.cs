using System;
using System.Reflection;
using eCentral.Core.Attributes;

namespace eCentral.Core
{
    /// <summary>
    /// Enum Extensions
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Will get the string value for a given enums value, this will
        /// only work if you assign the StringValue attribute to
        /// the items in your enum.
        /// </summary>
        /// <param name="value">
        /// </param>
        /// <returns>
        /// The get string value.
        /// </returns>
        public static string GetFriendlyName(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            if (fieldInfo != null)
            {
                // Get the stringvalue attributes
                var attribs = fieldInfo.GetCustomAttributes(typeof(FriendlyNameStringAttribute), false) as FriendlyNameStringAttribute[];

                // Return the first if there was a match.
                if (attribs != null)
                {
                    return attribs.Length > 0 ? attribs[0].FriendlyName : Enum.GetName(type, value);
                }
            }

            return Enum.GetName(type, value);
        }
    }
}