using Autofac;
using Autofac.Core;
using eCentral.Core.Infrastructure;
using eCentral.Core.Infrastructure.DependencyManagement;
using eCentral.Services.Security.Cryptography;

namespace eCentral.Services.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            // we need to set up the cryptography internal services
            builder.RegisterType<HashService>().As<IHashService>().InstancePerDependency();
            builder.RegisterType<AESService>().As<IAESService>().InstancePerDependency();
            builder.RegisterType<RijndaelEnhancedService>().As<IRijndaelEnhancedService>().InstancePerDependency();
        }

        public int Order
        {
            get { return 2; }
        }
    }
}
