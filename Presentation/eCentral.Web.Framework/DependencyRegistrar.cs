using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Integration.Mvc;
using eCentral.Core;
using eCentral.Core.Caching;
using eCentral.Core.Configuration;
using eCentral.Core.Data;
using eCentral.Core.Fakes;
using eCentral.Core.Infrastructure;
using eCentral.Core.Infrastructure.DependencyManagement;
using eCentral.Core.Plugins;
using eCentral.Core.Session;
using eCentral.Data;
using eCentral.Services.Authentication;
using eCentral.Services.Clients;
using eCentral.Services.Cms;
using eCentral.Services.Common;
using eCentral.Services.Companies;
using eCentral.Services.Configuration;
using eCentral.Services.Directory;
using eCentral.Services.Events;
using eCentral.Services.ExportImport;
using eCentral.Services.Helpers;
using eCentral.Services.Installation;
using eCentral.Services.Localization;
using eCentral.Services.Logging;
using eCentral.Services.Media;
using eCentral.Services.Messages;
using eCentral.Services.Security;
using eCentral.Services.Security.Cryptography;
using eCentral.Services.Tasks;
using eCentral.Services.Users;
using eCentral.Services.Web;
using eCentral.Web.Framework.EmbeddedViews;
using eCentral.Web.Framework.Mvc.Routes;
using eCentral.Web.Framework.UI;

namespace eCentral.Web.Framework
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //HTTP context and other related stuff
            builder.Register(c =>
                //register FakeHttpContext when HttpContext is not available
                HttpContext.Current != null ?
                (new HttpContextWrapper(HttpContext.Current) as HttpContextBase) :
                (new FakeHttpContext("~/") as HttpContextBase))
                .As<HttpContextBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Request)
                .As<HttpRequestBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Response)
                .As<HttpResponseBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Server)
                .As<HttpServerUtilityBase>()
                .InstancePerHttpRequest();
            builder.Register(c => c.Resolve<HttpContextBase>().Session)
                .As<HttpSessionStateBase>()
                .InstancePerHttpRequest();

            //web helper
            builder.RegisterType<WebHelper>().As<IWebHelper>().InstancePerHttpRequest();

            //controllers
            builder.RegisterControllers(typeFinder.GetAssemblies().ToArray());

            //data layer
            var dataSettingsManager = new DataSettingsManager();
            var dataProviderSettings = dataSettingsManager.LoadSettings();
            builder.Register(c => dataSettingsManager.LoadSettings()).As<DataSettings>();
            builder.Register(x => new EfDataProviderManager(x.Resolve<DataSettings>())).As<BaseDataProviderManager>().InstancePerDependency();


            builder.Register(x => (IEfDataProvider)x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IDataProvider>().InstancePerDependency();
            builder.Register(x => (IEfDataProvider)x.Resolve<BaseDataProviderManager>().LoadDataProvider()).As<IEfDataProvider>().InstancePerDependency();

            if (dataProviderSettings != null && dataProviderSettings.IsValid())
            {
                var efDataProviderManager = new EfDataProviderManager(dataSettingsManager.LoadSettings());
                var dataProvider = (IEfDataProvider)efDataProviderManager.LoadDataProvider();
                dataProvider.InitConnectionFactory();

                builder.Register<IDbContext>(c => new SiteObjectContext(dataProviderSettings.DataConnectionString)).InstancePerHttpRequest();
            }
            else
            {
                builder.Register<IDbContext>(c => new SiteObjectContext(dataSettingsManager.LoadSettings().DataConnectionString)).InstancePerHttpRequest();
            }


            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();

            //plugins
            builder.RegisterType<PluginFinder>().As<IPluginFinder>().InstancePerHttpRequest();

            //cache manager
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().Named<ICacheManager>("site_cache_static").SingleInstance();
            builder.RegisterType<PerRequestCacheManager>().As<ICacheManager>().Named<ICacheManager>("site_cache_per_request").InstancePerHttpRequest();

            //work context
            builder.RegisterType<WebWorkContext>().As<IWorkContext>().InstancePerHttpRequest();

            // session manager
            builder.RegisterType<SessionManager>().As<ISessionManager>().InstancePerLifetimeScope();

            //services
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerHttpRequest();

            builder.RegisterGeneric(typeof(ConfigurationProvider<>)).As(typeof(IConfigurationProvider<>));
            builder.RegisterSource(new SettingsSource());

            builder.RegisterType<UserService>().As<IUserService>().InstancePerHttpRequest();
            builder.RegisterType<UserRegistrationService>().As<IUserRegistrationService>().InstancePerHttpRequest();
            builder.RegisterType<UserReportService>().As<IUserReportService>().InstancePerHttpRequest();
            builder.RegisterType<PermissionService>().As<IPermissionService>().InstancePerHttpRequest();            

            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerHttpRequest(); // removed static cache as issue in edit, and context not found
            //.WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static")).InstancePerHttpRequest();
            builder.RegisterType<StateProvinceService>().As<IStateProvinceService>().InstancePerHttpRequest();
            //.WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static")).InstancePerHttpRequest();

            builder.RegisterType<CurrencyService>().As<ICurrencyService>().InstancePerHttpRequest();
            //.WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static")).InstancePerHttpRequest();

            //pass MemoryCacheManager to SettingService as cacheManager (cache settngs between requests)
            builder.RegisterType<SettingService>().As<ISettingService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static"))
                .InstancePerHttpRequest();

            builder.RegisterType<LocalizationService>().As<ILocalizationService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static"))
                .InstancePerHttpRequest();

            //pass MemoryCacheManager to LocalizedEntityService as cacheManager (cache locales between requests)
            builder.RegisterType<LocalizedEntityService>().As<ILocalizedEntityService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static"))
                .InstancePerHttpRequest();
            builder.RegisterType<LanguageService>().As<ILanguageService>().InstancePerHttpRequest();

            builder.RegisterType<FileDataService>().As<IFileDataService>().InstancePerHttpRequest();

            builder.RegisterType<ClientService>().As<IClientService>()
                .InstancePerHttpRequest();

            builder.RegisterType<CompanyService>().As<ICompanyService>()
                .InstancePerHttpRequest();
            builder.RegisterType<BranchOfficeService>().As<IBranchOfficeService>()
                .InstancePerHttpRequest();

            builder.RegisterType<MessageTemplateService>().As<IMessageTemplateService>().InstancePerHttpRequest();
            builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerHttpRequest();
            builder.RegisterType<EmailAccountService>().As<IEmailAccountService>().InstancePerHttpRequest();
            builder.RegisterType<WorkflowMessageService>().As<IWorkflowMessageService>().InstancePerHttpRequest();
            builder.RegisterType<MessageTokenProvider>().As<IMessageTokenProvider>().InstancePerHttpRequest();
            builder.RegisterType<Tokenizer>().As<ITokenizer>().InstancePerHttpRequest();
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerHttpRequest();
            builder.RegisterType<SmsService>().As<ISmsService>().InstancePerHttpRequest();

            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerHttpRequest();

            builder.RegisterType<FormsAuthenticationService>().As<IAuthenticationService>().InstancePerHttpRequest();

            builder.RegisterType<DefaultLogger>().As<ILogger>().InstancePerHttpRequest();
            builder.RegisterType<UserActivityService>().As<IUserActivityService>()
                //.WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static"))
                .InstancePerHttpRequest();

            builder.RegisterType<InstallationService>().As<IInstallationService>().InstancePerHttpRequest();

            builder.RegisterType<WidgetService>().As<IWidgetService>().InstancePerHttpRequest();

            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerHttpRequest();
            builder.RegisterType<PageTitleBuilder>().As<IPageTitleBuilder>().InstancePerHttpRequest();

            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerHttpRequest();

            builder.RegisterType<TelerikLocalizationServiceFactory>().As<Telerik.Web.Mvc.Infrastructure.ILocalizationServiceFactory>().InstancePerHttpRequest();

            builder.RegisterType<ExportManager>().As<IExportManager>().InstancePerHttpRequest();
            builder.RegisterType<ImportManager>().As<IImportManager>().InstancePerHttpRequest();

            //content
            builder.RegisterType<ContentService>().As<IContentService>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static")).InstancePerHttpRequest();

            builder.RegisterType<MobileDeviceHelper>().As<IMobileDeviceHelper>().InstancePerHttpRequest();
            
            builder.RegisterType<EmbeddedViewResolver>().As<IEmbeddedViewResolver>().SingleInstance();
            builder.RegisterType<RoutePublisher>().As<IRoutePublisher>().SingleInstance();

            // httpcacher and compresser
            builder.RegisterType<HttpResponseCacher>().As<IHttpResponseCacher>().InstancePerHttpRequest();
            builder.RegisterType<HttpResponseCompressor>().As<IHttpResponseCompressor>().InstancePerHttpRequest();
            builder.RegisterType<VirtualPathProviderWrapper>().As<IVirtualPathProvider>().SingleInstance();

            //Register event users
            var users = typeFinder.FindClassesOfType(typeof(IUser<>)).ToList();
            foreach (var user in users)
            {
                builder.RegisterType(user)
                    .As(user.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof(IUser<>)))
                    .InstancePerHttpRequest();
            }
            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();
        }

        public int Order
        {
            get { return 0; }
        }
    }


    public class SettingsSource : IRegistrationSource
    {
        static readonly MethodInfo BuildMethod = typeof(SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        [System.Diagnostics.DebuggerStepThrough]
        public IEnumerable<IComponentRegistration> RegistrationsFor(
                Service service,
                Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService;
            if (ts != null && typeof(ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration)buildMethod.Invoke(null, null);
            }
        }

        static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) => c.Resolve<IConfigurationProvider<TSettings>>().Settings)
                .InstancePerHttpRequest()
                .CreateRegistration();
        }

        public bool IsAdapterForIndividualComponents { get { return false; } }
    }

}
