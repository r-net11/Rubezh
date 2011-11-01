using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common;
using FiresecClient;
using System.Linq;
using PlansModule.Events;
using Infrastructure;

namespace PlansModule.ViewModels
{
    public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
    {
        public DeviceViewModel()
        {
            ServiceFactory.Events.GetEvent<DeviceInZoneChangedEvent>().Subscribe(x => { OnPropertyChanged("PresentationZone"); });
        }

        public void Initialize(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
        {
            Source = sourceDevices;
            Device = device;
        }

        public Device Device { get; private set; }

        public string PresentationZone
        {
            get
            {
                if (Device.Driver.IsZoneDevice)
                {
                    var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Device.ZoneNo);
                    if (zone != null)
                        return zone.PresentationName;
                }

                if (Device.Driver.IsZoneLogicDevice && Device.ZoneLogic != null)
                    return Device.ZoneLogic.ToString();

                if (Device.Driver.IsIndicatorDevice && Device.IndicatorLogic != null)
                    return Device.IndicatorLogic.ToString();

                return "";
            }
        }
    }
}
