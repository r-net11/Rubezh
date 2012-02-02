using System;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using PlansModule.ViewModels;

namespace PlansModule
{
    public class PlansModuleLoader
    {
        static PlansViewModel _plansViewModel;

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

        public static void CreateViewModels()
        {
            _plansViewModel = new PlansViewModel();
        }

        static void OnShowPlan(object obj)
        {
            ServiceFactory.Layout.Show(_plansViewModel);
        }

        static void OnShowDeviceOnPlan(Guid deviceUID)
        {
            ServiceFactory.Layout.Show(_plansViewModel);
            _plansViewModel.ShowDevice(deviceUID);
        }

        static void OnShowZoneOnPlan(ulong zoneNo)
        {
            ServiceFactory.Layout.Show(_plansViewModel);
            _plansViewModel.ShowZone(zoneNo);
        }
    }
}