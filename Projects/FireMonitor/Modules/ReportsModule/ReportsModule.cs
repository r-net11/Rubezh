using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;
using ReportsModule.ViewModels;

namespace ReportsModule
{
    public class ReportsModule : IModule
    {
        public ReportsModule()
        {
            ServiceFactory.Events.GetEvent<ShowReportsEvent>().Subscribe(OnShowReports);
        }
        
        public void Initialize()
        {
            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void CreateViewModels()
        {
            reportsViewModel = new ReportsViewModel();
        }

        static ReportsViewModel reportsViewModel;

        static void OnShowReports(object obj)
        {
            reportsViewModel.Initialize();
            ServiceFactory.Layout.Show(reportsViewModel);
        }
    }
}