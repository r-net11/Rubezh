using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using ReportsModule.ViewModels;
using FiresecAPI.Models;

namespace ReportsModule
{
    public class ReportsModuleLoader
    {
        static ReportsViewModel ReportsViewModel;

        public ReportsModuleLoader()
        {
            ServiceFactory.Events.GetEvent<ShowReportsEvent>().Subscribe(OnShowReports);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        public static void Initialize()
        {
            //ReportsViewModel.Initialize();
        }

        static void CreateViewModels()
        {
            ReportsViewModel = new ReportsViewModel();

            //ReportsViewModel.SelectedReportName = ReportType.ReportDevicesList;
        }

        static void OnShowReports(object obj)
        {
            ServiceFactory.Layout.Show(ReportsViewModel);
        }
    }
}