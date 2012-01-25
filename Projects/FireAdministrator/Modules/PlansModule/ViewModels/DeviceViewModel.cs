using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
    {
        public DeviceViewModel(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
        {
            ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
            ServiceFactory.Events.GetEvent<DeviceInZoneChangedEvent>().Subscribe(x => { OnPropertyChanged("PresentationZone"); });
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

        public bool IsOnPlan
        {
            get
            {
                return Device.PlanElementUIDs.Count > 0;
            }
        }

        public void Update()
        {
            OnPropertyChanged("IsOnPlan");
        }

        public RelayCommand ShowOnPlanCommand { get; private set; }
        void OnShowOnPlan()
        {
            if (Device.PlanElementUIDs.Count > 0)
            {
                ServiceFactory.Events.GetEvent<ShowElementDeviceEvent>().Publish(Device.PlanElementUIDs[0]);
            }
        }
    }
}