using System;

namespace eCentral.Core.Attributes
{
    /// <summary>
    /// This attribute is used to represent a string value
    /// for a value in an enum.
    /// </summary>
    public partial class FriendlyNameStringAttribute : Attribute
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendlyStringAttribute"/> class. 
        /// Constructor used to init a StringValue Attribute
        /// </summary>
        /// <param name="value">
        /// </param>
        public FriendlyNameStringAttribute(string value)
        {
            FriendlyName = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Holds the stringvalue for a value in an enum.
        /// </summary>
        public string FriendlyName
        {
            get;
            protected set;
        }

        #endregion
        
    }
}
