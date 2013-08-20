using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Validators.Users;
using FluentValidation.Attributes;

namespace eCentral.Web.Models.Users
{
    [Validator(typeof(UserValidator))]
    public partial class UserModel : BaseAuditHistoryModel
    {
        public UserModel()
        {
            this.AvailableOffices = new List<SelectListItem>();
        }

        [SiteResourceDisplayName("Users.Fields.Username")]
        [AllowHtml]
        public string Username { get; set; }

        [SiteResourceDisplayName("Users.Fields.FirstName")]
        public string FirstName { get; set; }

        [SiteResourceDisplayName("Users.Fields.LastName")]
        public string LastName { get; set; }

        [SiteResourceDisplayName("Users.Fields.Mobile")]
        public string Mobile { get; set; }

        public string CreatedOn { get; set; }

        public string LastActivityDate { get; set; }

        public string UserRoleNames { get; set; }

        /// <summary>
        /// Gets or sets whether the user is an administrator
        /// </summary>
        [SiteResourceDisplayName("Users.Fields.Administrator")]
        public bool IsAdministrator { get; set; }

        [SiteResourceDisplayName("Users.Fields.Offices")]
        public string OfficeId { get; set; }

        public IList<SelectListItem> AvailableOffices { get; set; }
    }
}