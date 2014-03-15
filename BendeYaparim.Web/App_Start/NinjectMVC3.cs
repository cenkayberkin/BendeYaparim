[assembly: WebActivator.PreApplicationStartMethod(typeof(BendeYaparim.Web.App_Start.NinjectMVC3), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(BendeYaparim.Web.App_Start.NinjectMVC3), "Stop")]

namespace BendeYaparim.Web.App_Start
{
    using System.Reflection;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Mvc;
    using BendeYaparim.Web.DAL;
    using BendeYaparim.Web.Models;
    using BendeYaparim.Web.Infrastructure;
    using System.Web.Security;

    public static class NinjectMVC3
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestModule));
            DynamicModuleUtility.RegisterModule(typeof(HttpApplicationInitializationModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            RegisterServices(kernel);

            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<BendeyaparimContext>().ToSelf().InRequestScope();
            kernel.Bind<ICategoryRepository>().To<CategoryRepository>().InRequestScope();
            kernel.Bind<IJobSeekRepository>().To<JobSeekRepository>().InRequestScope();
            kernel.Bind<IJobOfferRepository>().To<JobOfferRepository>().InRequestScope();
            kernel.Bind<UserRepository>().ToSelf().InRequestScope();
            kernel.Bind<ICityRepository>().To<CityRepository>().InRequestScope();
            kernel.Bind<IFormsAuthenticationService>().To<FormsAuthenticationService>().InRequestScope();
            kernel.Bind<IMembershipService>().To<AccountMembershipService>().InRequestScope();
            kernel.Bind<MembershipProvider>().To<MyMembershipProvider>().InRequestScope();
            kernel.Bind<IRoleRepository>().To<RoleRepository>().InRequestScope();
            kernel.Bind<IMessageRepository>().To<MessageRepository>().InRequestScope();
        }
    }
}
