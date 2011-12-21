using System.Windows;
using Infrastructure.Common;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure
{
    public class ServiceFactory
    {
        public static void Initialize(ILayoutService ILayoutService, IUserDialogService IUserDialogService)
        {
            Events = new EventAggregator();
            ResourceService = new ResourceService();
            Layout = ILayoutService;
            UserDialogs = IUserDialogService;
        }

        public static IEventAggregator Events { get; private set; }
        public static ResourceService ResourceService { get; private set; }
        public static ILayoutService Layout { get; private set; }
        public static IUserDialogService UserDialogs { get; private set; }

        public static Window ShellView { get; set; }
    }
}