using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;
using PlansModule.ViewModels;

namespace PlansModule
{
    public class PlansModule : IModule
    {
        static PlansViewModel _plansViewModel;
        public static bool HasChanges { get; set; }

        public PlansModule()
        {
            ServiceFactory.Events.GetEvent<ShowPlansEvent>().Subscribe(OnShowPlans);
        }

        public void Initialize()
        {
            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Designer/DesignerCanvas.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Designer/DesignerItem.xaml"));
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "Designer/Rectangle/ResizeChrome.xaml"));
        }

        static void CreateViewModels()
        {
            _plansViewModel = new PlansViewModel();
        }

        static void OnShowPlans(string obj)
        {
            _plansViewModel.Initialize();
            ServiceFactory.Layout.Show(_plansViewModel);
        }

        public static void Save()
        {
            _plansViewModel.PlanDesignerViewModel.Save();
        }
    }
}