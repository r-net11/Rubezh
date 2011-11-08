using System.Windows;
using Infrastructure.Common;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure
{
    public class ServiceFactory
    {
        public static void Initialize(ILayoutService ILayoutService, IUserDialogService IUserDialogService, IResourceService IResourceService, ISecurityService ISecurityService)
        {
            Events = new EventAggregator();
            Layout = ILayoutService;
            UserDialogs = IUserDialogService;
            ResourceService = IResourceService;
            SecurityService = ISecurityService;
        }

        public static IEventAggregator Events { get; private set; }
        public static ILayoutService Layout { get; private set; }
        public static IUserDialogService UserDialogs { get; private set; }
        public static IResourceService ResourceService { get; private set; }
        public static ISecurityService SecurityService { get; private set; }

        public static Window ShellView { get; set; }
    }

    //public class ServiceFactory
    //{
    //    static IUnityContainer _container;
    //    static IUnityContainer Container
    //    {
    //        get
    //        {
    //            if (_container == null)
    //            {
    //                _container = ServiceLocator.Current.GetInstance<IUnityContainer>();
    //            }
    //            return _container;
    //        }
    //    }

    //    public static ILayoutService Layout
    //    {
    //        get { return Get<ILayoutService>(); }
    //    }

    //    public static IEventAggregator Events
    //    {
    //        get { return Get<IEventAggregator>(); }
    //    }

    //    public static IUserDialogService UserDialogs
    //    {
    //        get { return Get<IUserDialogService>(); }
    //    }

    //    public static void RegisterType<TFrom, TTo>()
    //        where TTo : TFrom
    //    {
    //        RegisterType<TFrom, TTo>(false);
    //    }

    //    public static void RegisterType<TFrom, TTo>(bool registerAsSingletone)
    //        where TTo : TFrom
    //    {
    //        if (registerAsSingletone)
    //            Container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
    //        else
    //            Container.RegisterType<TFrom, TTo>();
    //    }

    //    public static void RegisterInstance<TInterface>(TInterface instance)
    //    {
    //        Container.RegisterInstance(instance);
    //    }

    //    public static TInterface Get<TInterface>()
    //    {
    //        return Container.Resolve<TInterface>();
    //    }
    //}
}