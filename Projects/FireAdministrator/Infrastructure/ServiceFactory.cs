using System.Windows;
using Infrastructure.Common;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure
{
    public class ServiceFactory
    {
        public static void Initialize(ILayoutService ILayoutService, IUserDialogService IUserDialogService, IProgressService IProgressService)
        {
            SaveService = new SaveService();
            Events = new EventAggregator();
            ResourceService = new ResourceService();
            Layout = ILayoutService;
            UserDialogs = IUserDialogService;
            ProgressService = IProgressService;
        }

        public static AppSettings AppSettings { get; set; }

        public static SaveService SaveService { get; private set; }
        public static IEventAggregator Events { get; private set; }
        public static ResourceService ResourceService { get; private set; }
        public static ILayoutService Layout { get; private set; }
        public static IUserDialogService UserDialogs { get; private set; }
        public static IProgressService ProgressService { get; private set; }

        public static Window ShellView { get; set; }
    }
}