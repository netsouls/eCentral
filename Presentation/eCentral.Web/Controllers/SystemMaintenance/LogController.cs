using System;
using System.Linq;
using System.Web.Mvc;
using eCentral.Core;
using eCentral.Core.Domain.Logging;
using eCentral.Core.Domain.Security;
using eCentral.Core.Domain.Users;
using eCentral.Services.Helpers;
using eCentral.Services.Localization;
using eCentral.Services.Logging;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Models.Logging;

namespace eCentral.Web.Controllers.SystemMaintenance
{
    [RoleAuthorization(Role = SystemUserRoleNames.Administrators)]
    [PermissionAuthorization(Permission = SystemPermissionNames.ManageMaintenance)]
    public class LogController : BaseController
    {
        #region Fields

        private readonly ILogger logger;
        private readonly IDateTimeHelper dateTimeHelper;
        private readonly ILocalizationService localizationService;
        private readonly IWorkContext workContext;
        
        #endregion

        #region Ctor

        public LogController(ILogger logger, ILocalizationService localizationService,
            IWorkContext workContext, IDateTimeHelper dateTimeHelper)
        {
            this.localizationService = localizationService;
            this.logger              = logger;
            this.workContext         = workContext;
            this.dateTimeHelper      = dateTimeHelper;
        }

        #endregion

        public ActionResult Index() 
        {
            var logSearchModel = new LogSearchModel();

            // add all type of log levels
            logSearchModel.AvailableLogLevels = LogLevel.Debug.ToSelectList(false).ToList();
            logSearchModel.AvailableLogLevels.Insert(0, new SelectListItem() { Text = localizationService.GetResource("Common.All"), Value = "0" });

            return View(logSearchModel);
        }

        [HttpPost]
        public ActionResult List(LogSearchModel model)
        {
            if (!Request.IsAjaxRequest())
                return RedirectToAction(SystemRouteNames.Index);

            DateTime? startDateValue = (model.CreatedOnFrom == null) ? null
                : (DateTime?)dateTimeHelper.ConvertToUtcTime(model.CreatedOnFrom.Value, dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.CreatedOnTo == null) ? null
                            : (DateTime?)dateTimeHelper.ConvertToUtcTime(model.CreatedOnTo.Value, dateTimeHelper.CurrentTimeZone).AddDays(1);

            LogLevel? logLevel = model.LogLevelId > 0 ? (LogLevel?)(model.LogLevelId) : null;

            var logs = logger.GetAll(startDateValue, endDateValue,
                model.Message, logLevel, CommonHelper.To<int>(Request[StateKeyManager.DataTable.DISPLAY_START]) / CommonHelper.To<int>(Request[StateKeyManager.DataTable.DISPLAY_LENGTH]),
                CommonHelper.To<int>(Request[StateKeyManager.DataTable.DISPLAY_LENGTH]));

            var pagedModel = new PagedModel<LogModel>
            {
                Data = logs.Select(x =>
                {
                    return new LogModel()
                    {
                        RowId = x.RowId,
                        LogLevel = x.LogLevel.GetLocalizedEnum(localizationService, workContext),
                        ShortMessage = x.ShortMessage,
                        FullMessage = x.FullMessage,
                        IpAddress = x.IpAddress,
                        UserId = x.UserId,
                        UserName = x.User != null ? x.User.Username : null,
                        PageUrl = x.PageUrl,
                        ReferrerUrl = x.ReferrerUrl,
                        CreatedOn = x.CreatedOn.ToString(StateKeyManager.DateTimeFormat)
                    };
                }),
                TotalCount = logs.TotalCount
            };

            return Json(new DataTablesPagedParser<LogModel>(Request, pagedModel).Parse());
        }

        public ActionResult View(Guid rowId)
        {
            var log = logger.GetById(rowId);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction(SystemRouteNames.Index);

            var model = new LogModel()
            {
                RowId = log.RowId,
                LogLevel = log.LogLevel.GetLocalizedEnum(localizationService, workContext),
                ShortMessage = log.ShortMessage,
                FullMessage = log.FullMessage,
                IpAddress = log.IpAddress,
                UserId = log.UserId,
                UserName = log.User != null ? log.User.Username : null,
                PageUrl = log.PageUrl,
                ReferrerUrl = log.ReferrerUrl,
                CreatedOn = log.CreatedOn.ToString(StateKeyManager.DateTimeFormat)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ClearAll()
        {
            //logger.ClearLog();

            SuccessNotification(localizationService.GetResource("System.Log.Cleared"));
            return RedirectToAction(SystemRouteNames.Index);
        }

        [HttpPost]
        public ActionResult DeleteSelected(Guid[] rowIds)
        {
            if (rowIds != null)
            {
                var logItems = logger.GetByIds(rowIds);
                foreach (var logItem in logItems)
                    logger.Delete(logItem);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult Delete(Guid rowId)
        {
            System.Threading.Thread.Sleep(1000);
            var log = logger.GetById(rowId);
            if (log == null)
                //No log found with the specified id
                return RedirectToAction(SystemRouteNames.Index);

            logger.Delete(log);
            SuccessNotification(localizationService.GetResource("System.Log.Deleted"));
            return RedirectToAction(SystemRouteNames.Index);
        }
    }
}