using System;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using PlansModule.ViewModels;

namespace PlansModule
{
    public class PlansModuleLoader
    {
        static PlansViewModel PlansViewModel;

        public PlansModuleLoader()
        {
            ServiceFactory.Events.GetEvent<ShowPlansEvent>().Subscribe(OnShowPlan);
            ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Subscribe(OnShowDeviceOnPlan);
            ServiceFactory.Events.GetEvent<ShowZoneOnPlanEvent>().Subscribe(OnShowZoneOnPlan);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        public static void Initialize()
        {
            PlansViewModel.Initialize();
        }

        public static void CreateViewModels()
        {
            PlansViewModel = new PlansViewModel();
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

        static void OnShowZoneOnPlan(ulong zoneNo)
        {
            ServiceFactory.Layout.Show(PlansViewModel);
            PlansViewModel.ShowZone(zoneNo);
        }
    }
}