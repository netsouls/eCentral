using System;

namespace eCentral.Core.Domain.Configuration
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    public partial class Setting : BaseEntity
    {
        public Setting() { }
        
        public Setting(string name, string value) {
            this.Name = name;
            this.Value = value;
        }
        
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets or sets the date and time on record added
        /// </summary>
        public virtual DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public virtual DateTime UpdatedOn{ get; set; }

        /// <summary>
        /// Returns the setting value as the specified type
        /// </summary>
        public virtual T As<T>() {
            return CommonHelper.To<T>(this.Value);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
