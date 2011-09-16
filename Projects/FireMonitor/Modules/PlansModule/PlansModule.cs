using System;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Modularity;
using PlansModule.ViewModels;

namespace PlansModule
{
    public class PlansModule : IModule
    {
        static PlansViewModel PlansViewModel;

        public PlansModule()
        {
            ServiceFactory.Events.GetEvent<ShowPlanEvent>().Subscribe(OnShowPlan);
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Subscribe(OnShowDeviceOnPlan);
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
            PlansViewModel = new PlansViewModel();
            PlansViewModel.Initialize();
        }

        static void OnShowPlan(object obj)
        {
            ServiceFactory.Layout.Show(PlansViewModel);
        }

        static void OnShowDeviceOnPlan(Guid deviceUID)
        {
            ServiceFactory.Layout.Show(PlansViewModel);
            PlansViewModel.ShowDevice(deviceUID);
        }
    }
}