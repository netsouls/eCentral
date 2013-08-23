using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using eCentral.Core;
using eCentral.Core.Data;
using eCentral.Core.Domain;
using eCentral.Core.Infrastructure;
using eCentral.Services.Logging;
using eCentral.Services.Tasks;
using eCentral.Web.Framework;
using eCentral.Web.Framework.Controllers;
using eCentral.Web.Framework.EmbeddedViews;
using eCentral.Web.Framework.Mvc;
using eCentral.Web.Framework.Mvc.Routes;
using eCentral.Web.Framework.Themes;
using eCentral.Web.Framework.Validators;
using FluentValidation.Mvc;
//using StackExchange.Profiling;
//using StackExchange.Profiling.MVCHelpers;

namespace eCentral.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //do not register HandleErrorAttribute. use classic error handling mode
            //filters.Add(new HandleErrorAttribute());
            // register ELMAH
            filters.Add(new HandleErrorWithELMAHAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*allaxd}", new { allaxd = @".*\.axd(/.*)?" });
            routes.IgnoreRoute("{*allashx}", new { allashx = @".*\.ashx(/.*)?" });

            //register custom routes (plugins, etc)
            var routePublisher = EngineContext.Current.Resolve<IRoutePublisher>();
            routePublisher.RegisterRoutes(routes);

            /*routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{rowId}", // URL with parameters
                new { controller = "Home", action = "Index", area = "", rowId = UrlParameter.Optional },
                new[] { "eCentral.Web.Controllers" }
            );*/
        }

        protected void Application_Start()
        {
            //initialize engine context
            EngineContext.Initialize(false);

            bool databaseInstalled = DataSettingsHelper.DatabaseIsInstalled();

            //set dependency resolver
            var dependencyResolver = new SiteDependencyResolver();
            DependencyResolver.SetResolver(dependencyResolver);

            //model binders
            ModelBinders.Binders.Add(typeof(BaseModel), new SiteModelBinder());

            if (databaseInstalled)
            {
                //remove all view engines
                ViewEngines.Engines.Clear();
                //except the themeable razor view engine we use
                ViewEngines.Engines.Add(new ThemeableRazorViewEngine());
            }

            //Add some functionality on top of the default ModelMetadataProvider
            ModelMetadataProviders.Current = new SiteMetadataProvider();

            //Registering some regular mvc stuf
            AreaRegistration.RegisterAllAreas();

            /*if (databaseInstalled &&
                EngineContext.Current.Resolve<SiteInformationSettings>().DisplayMiniProfilerInPublicSite)
            {
                GlobalFilters.Filters.Add(new ProfilingActionFilter());
            }*/
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            FluentValidationModelValidatorProvider.Configure(
                provider =>
                {

                    provider.AddImplicitRequiredValidator = false;
                    provider.ValidatorFactory = new SiteValidatorFactory();

                    // add custom validators
                    provider.Add(typeof(IsCheckedPropertyValidator), (metadata, context, rule, validator) =>
                        new IsCheckedFluentValidationPropertyValidator(metadata, context, rule, validator));

                    provider.Add(typeof(IsUniquePropertyValidator), (metadata, context, rule, validator) =>
                        new IsUniqueFluentValidationPropertyValidator(metadata, context, rule, validator));

                    provider.Add(typeof(NotMatchesPropertyValidator), (metadata, context, rule, validator) =>
                        new NotMatchesFluentValidationPropertyValidator(metadata, context, rule, validator));
                }
            );

            //register virtual path provider for embedded views
            var embeddedViewResolver = EngineContext.Current.Resolve<IEmbeddedViewResolver>();
            var embeddedProvider = new EmbeddedViewVirtualPathProvider(embeddedViewResolver.GetEmbeddedViews());
            HostingEnvironment.RegisterVirtualPathProvider(embeddedProvider);

            //mobile device support - will requried to be added later

            //start scheduled tasks
            if (databaseInstalled)
            {
                TaskManager.Instance.Initialize();
                TaskManager.Instance.Start();
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            EnsureDatabaseIsInstalled();

            /*if (DataSettingsHelper.DatabaseIsInstalled() &&
                EngineContext.Current.Resolve<SiteInformationSettings>().DisplayMiniProfilerInPublicSite)
            {
                //MiniProfiler.Start(ProfileLevel.Verbose);
            }*/
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            /*if (DataSettingsHelper.DatabaseIsInstalled() &&
                EngineContext.Current.Resolve<SiteInformationSettings>().DisplayMiniProfilerInPublicSite)
            {
                //stop as early as you can, even earlier with MvcMiniProfiler.MiniProfiler.Stop(discardResults: true);
                //MiniProfiler.Stop();
            }*/
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            //we don't do it in Application_BeginRequest because a user is not authenticated yet
            SetWorkingCulture();
        }

        protected void Application_Error(Object sender, EventArgs e) //NOTADDED
        {
            //disable compression (if enabled). More info - http://stackoverflow.com/questions/3960707/asp-net-mvc-weird-characters-in-error-page
            CompressAttribute.DisableCompression(HttpContext.Current);
            //log error
            LogException(Server.GetLastError());
        }

        protected void EnsureDatabaseIsInstalled()
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            string installUrl = string.Format("{0}install", webHelper.AbsoluteWebRoot.ToString());
            if (!webHelper.IsStaticResource(this.Request) &&
                !DataSettingsHelper.DatabaseIsInstalled() &&
                !webHelper.GetThisPageUrl(false).StartsWith(installUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                this.Response.Redirect(installUrl);
            }
        }

        protected void SetWorkingCulture()
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            if (webHelper.IsStaticResource(this.Request))
                return;

            if (webHelper.GetThisPageUrl(false).StartsWith(string.Format("{0}webadmin", webHelper.AbsoluteWebRoot),
                StringComparison.InvariantCultureIgnoreCase))
            {
                //admin area
                //always set culture to 'en-US'
                //we set culture of admin area to 'en-US' because current implementation of Telerik grid 
                //doesn't work well in other cultures
                //e.g., editing decimal value in russian culture
                var culture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            else
            {
                //public store
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                if (workContext.CurrentUser != null && workContext.WorkingLanguage != null)
                {
                    var culture = new CultureInfo(workContext.WorkingLanguage.LanguageCulture);
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
            }
        }

        protected void LogException(Exception exc)
        {
            if (exc == null)
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            try
            {
                var logger = EngineContext.Current.Resolve<ILogger>();
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                logger.Error(exc.Message, exc, workContext.CurrentUser);

                // raise elmah
                //Elmah.ErrorSignal.FromCurrentContext().Raise(exc);
            }
            catch (Exception)
            {
                //don't throw new exception if occurs
            }
        }
    }
}