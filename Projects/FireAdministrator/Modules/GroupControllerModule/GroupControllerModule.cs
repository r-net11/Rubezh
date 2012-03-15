using GroupControllerModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using GroupControllerModule.Converter;

namespace GroupControllerModule
{
    public class GroupControllerModule
    {
        static DevicesViewModel _devicesViewModel;
        static ZonesViewModel _zonesViewModel;

        public GroupControllerModule()
        {
            ServiceFactory.Events.GetEvent<ShowXDevicesEvent>().Unsubscribe(OnShowXDevices);
            ServiceFactory.Events.GetEvent<ShowXZonesEvent>().Unsubscribe(OnShowXZones);
            ServiceFactory.Events.GetEvent<ShowXDevicesEvent>().Subscribe(OnShowXDevices);
            ServiceFactory.Events.GetEvent<ShowXZonesEvent>().Subscribe(OnShowXZones);

            var configurationConverter = new ConfigurationConverter();
            configurationConverter.Convert();

            RegisterResources();
            CreateViewModels();
        }

        void RegisterResources()
        {
            ServiceFactory.ResourceService.AddResource(new ResourceDescription(GetType().Assembly, "DataTemplates/Dictionary.xaml"));
        }

        void CreateViewModels()
        {
            _devicesViewModel = new DevicesViewModel();
            _zonesViewModel = new ZonesViewModel();
        }

        static void OnShowXDevices(object obj)
        {
            ServiceFactory.Layout.Show(_devicesViewModel);
        }

        static void OnShowXZones(object obj)
        {
            ServiceFactory.Layout.Show(_zonesViewModel);
        }
    }
}