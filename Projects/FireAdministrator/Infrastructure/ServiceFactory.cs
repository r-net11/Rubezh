using System.Windows;
using Infrastructure.Common;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure
{
    public class ServiceFactory
    {
        public static void Initialize(ILayoutService ILayoutService, IUserDialogService IUserDialogService, IResourceService IResourceService)
        {
            Events = new EventAggregator();
            Layout = ILayoutService;
            UserDialogs = IUserDialogService;
            ResourceService = IResourceService;
        }

        public static IEventAggregator Events { get; private set; }
        public static ILayoutService Layout { get; private set; }
        public static IUserDialogService UserDialogs { get; private set; }
        public static IResourceService ResourceService { get; private set; }

        public static Window ShellView { get; set; }
    }
}