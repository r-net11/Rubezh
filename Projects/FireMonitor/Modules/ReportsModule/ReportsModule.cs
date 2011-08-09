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
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        void CreateViewModels()
        {
            reportsViewModel = new ReportsViewModel();
            reportsViewModel.Initialize();
        }

        static ReportsViewModel reportsViewModel;

        static void OnShowReports(object obj)
        {
            ServiceFactory.Layout.Show(reportsViewModel);
        }
    }
}