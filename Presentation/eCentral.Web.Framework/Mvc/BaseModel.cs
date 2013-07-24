using System;
using System.Web.Mvc;

namespace eCentral.Web.Framework.Mvc
{
    public class BaseModel
    {
        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }
    }

    public class BaseEntityModel : BaseModel
    {
        public virtual Guid RowId { get; set; }

        public virtual bool IsEdit { get; set; }
    }

    public class BaseAuditHistoryModel : BaseEntityModel, IAudiHistoryModel
    {
        /// <summary>
        /// Get or set the user and time when the entity was last update by and on 
        /// </summary>
        public virtual string LastUpdated { get; set; }

        /// <summary>
        /// Get or set the pulishing status of the entity
        /// </summary>
        public virtual string PublishingStatus { get; set; }
    }

    /// <summary>
    /// Interface for audit history model
    /// </summary>
    public interface IAudiHistoryModel : IPublishingModel
    {
        /// <summary>
        /// Get or set the user and time when the entity was last update by and on 
        /// </summary>
        string LastUpdated { get; set; }
    }

    /// <summary>
    /// Interface for publishing status model
    /// </summary>
    public interface IPublishingModel
    {
        /// <summary>
        /// Get or set the pulishing status of the entity
        /// </summary>
        string PublishingStatus { get; set; }
    }
}