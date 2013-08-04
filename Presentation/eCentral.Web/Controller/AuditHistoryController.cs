using System;
using eCentral.Core.Domain.Logging;
using System.Linq;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Domain.Users;
using eCentral.Services.Helpers;
using eCentral.Services.Localization;
using eCentral.Services.Logging;
using eCentral.Services.Users;
using eCentral.Web.Extensions;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Infrastructure.Cache;
using eCentral.Web.Models.Logging;

namespace eCentral.Web.Controllers
{
    [RoleAuthorization(Role = SystemUserRoleNames.Administrators)]
    public class AuditHistoryController : BaseController
    {
        #region Fields

        private readonly IWorkContext workContext;
        private readonly IUserService userService;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly ILocalizationService localizationService;
        private readonly ICacheManager cacheManager;
        private readonly IUserActivityService activityService;
        private readonly IWebHelper webHelper;

        #endregion

        #region Ctor

        public AuditHistoryController(IWorkContext workContext, ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper, IUserActivityService activityService,
            IUserService userService, IWebHelper webHelper, ICacheManager cacheManager)
        {
            this.workContext         = workContext;
            this.localizationService = localizationService;
            this.webHelper           = webHelper;
            this.userService         = userService;
            this.dateTimeHelper      = dateTimeHelper;
            this.cacheManager        = cacheManager;
            this.activityService     = activityService;
        }

        #endregion

        #region History Logs

        public ActionResult Index()
        {
            var activityLogSearchModel = new ActivityLogSearchModel();

            // add all type of activities
            activityLogSearchModel.ActivityLogType.Add(new SelectListItem()
            {
                Value = Guid.Empty.ToString(),
                Text = "All"
            });

            activityService.GetAllActivityTypes()
                .OrderBy(at => at.Name)
                .Select(at =>
                {
                    return new SelectListItem()
                    {
                        Value = at.RowId.ToString(),
                        Text = at.Name
                    };
                })
                .ForEach (item => {
                    activityLogSearchModel.ActivityLogType.Add(item);
                });

            activityLogSearchModel.Users.Add(new SelectListItem()
            {
                Value = Guid.Empty.ToString(), Text="All"
            });

            activityLogSearchModel.Users.AddRange(
                this.PrepareUserSelectList(userService, cacheManager, Guid.Empty, PublishingStatus.All));

            return View(activityLogSearchModel);
        }

        [HttpPost]
        public ActionResult List(ActivityLogSearchModel model)
        {
            if (!Request.IsAjaxRequest())
                return RedirectToAction(SystemRouteNames.Index);

            DateTime? startDateValue = (model.CreatedOnFrom == null) ? null
                : (DateTime?)dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, dateTimeHelper.CurrentTimeZone).AddDays(1);

            string cacheKey = ModelCacheEventUser.AUDITHISTORY_MODEL_KEY.FormatWith(
                "List.{0}.{1}.{2}.{3}".FormatWith((startDateValue.HasValue ? startDateValue.ToString() : string.Empty),
                    (endDateValue.HasValue ? endDateValue.ToString() : string.Empty), model.ActivityLogTypeId.ToString(), model.UserId.ToString()));

            var cacheModel = cacheManager.Get(cacheKey, () =>
            {
                var activityLogs =
                    activityService.GetAllActivities(model.CreatedOnFrom, model.CreatedOnTo, model.UserId, model.ActivityLogTypeId)
                    .Select(ah =>
                    {
                        var activitylogModel = ah.ToModel();
                        activitylogModel.CreatedOn = ah.CreatedOn.ToString(StateKeyManager.DateTimeFormat);
                        return activitylogModel;
                    });

                return activityLogs;
            });

            return Json(new DataTablesParser<ActivityLogModel>(Request, cacheModel ).Parse());
        }

        #endregion

        #region Activity log types

        public ActionResult CreateActivityLogType()
        {
            var model = new ActivityLogTypeModel();

            return PartialView("_CreateActivityLogType", model);
        }

        [HttpPost]
        public ActionResult CreateActivityLogType(ActivityLogTypeModel model )
        {
            if (ModelState.IsValid)
            {
                var activityType = new ActivityLogType()
                {
                    Name = model.Name.Trim(),
                    SystemKeyword = model.SystemKeyword.Trim()
                };

                activityService.InsertActivityType(activityType);
                return Json(new { IsValid = true });
            }

            //If we got this far, something failed, redisplay form
            return Json(new { IsValid = false, htmlData = RenderPartialViewToString("_CreateActivityLogType", model) });
        }

        #endregion
    }
}
