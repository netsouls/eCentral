using Autofac;
using Autofac.Core;
using eCentral.Core.Caching;
using eCentral.Core.Infrastructure;
using eCentral.Core.Infrastructure.DependencyManagement;
using eCentral.Web.Controllers;
using eCentral.Web.Controllers.Administration;

namespace eCentral.Web.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //we cache presentation models between requests
            builder.RegisterType<ClientController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static"));

            builder.RegisterType<CompanyController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static"));

            builder.RegisterType<BranchOfficeController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static"));

            builder.RegisterType<UserController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static"));

            builder.RegisterType<CommonController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static"));
            builder.RegisterType<APIController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("site_cache_static"));
        }

        public int Order
        {
            get { return 2; }
        }
    }
}
