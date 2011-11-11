using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using ReportsModule.ViewModels;

namespace ReportsModule
{
    public class ReportsModule
    {
        static ReportsViewModel ReportsViewModel;

        public ReportsModule()
        {
            ServiceFactory.Events.GetEvent<ShowReportsEvent>().Subscribe(OnShowReports);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void CreateViewModels()
        {
            ReportsViewModel = new ReportsViewModel();
        }

        static void OnShowReports(object obj)
        {
            ServiceFactory.Layout.Show(ReportsViewModel);
        }
    }
}