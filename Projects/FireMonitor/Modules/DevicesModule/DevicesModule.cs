using System;
using DevicesModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace DevicesModule
{
    public class DevicesModule
    {
        static DevicesViewModel DevicesViewModel;
        static ZonesViewModel ZonesViewModel;

        public DevicesModule()
        {
            ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Subscribe(OnShowDevice);
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Subscribe(OnShowZone);
            ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Subscribe(OnShowDeviceDetails);

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        static void CreateViewModels()
        {
            DevicesViewModel = new DevicesViewModel();
            ZonesViewModel = new ZonesViewModel();
        }

        static void OnShowDevice(Guid deviceUID)
        {
            DevicesViewModel.Select(deviceUID);
            ServiceFactory.Layout.Show(DevicesViewModel);
        }

        static void OnShowZone(ulong? zoneNo)
        {
            ZonesViewModel.Select(zoneNo);
            ServiceFactory.Layout.Show(ZonesViewModel);
        }

        static void OnShowDeviceDetails(Guid deviceUID)
        {
            ServiceFactory.UserDialogs.ShowWindow(new DeviceDetailsViewModel(deviceUID));
        }
    }
}