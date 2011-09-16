using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;
using PlansModule.ViewModels;

namespace PlansModule
{
    public class PlansModule : IModule
    {
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
            var resourceService = ServiceFactory.Get<IResourceService>();
            resourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void CreateViewModels()
        {
            plansViewModel = new PlansViewModel();
            plansViewModel.Initialize();
        }

        static PlansViewModel plansViewModel;

        static void OnShowPlans(string obj)
        {
            ServiceFactory.Layout.Show(plansViewModel);
        }
    }
}