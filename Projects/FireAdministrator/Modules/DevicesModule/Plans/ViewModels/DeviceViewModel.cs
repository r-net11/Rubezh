using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrustructure.Plans.Events;

namespace DevicesModule.Plans.ViewModels
{
    public class DeviceViewModel : TreeBaseViewModel<DeviceViewModel>
    {
        public DeviceViewModel(Device device, ObservableCollection<DeviceViewModel> sourceDevices)
        {
            ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			//ServiceFactory.Events.GetEvent<DeviceInZoneChangedEvent>().Subscribe(x => { OnPropertyChanged("PresentationZone"); });
            Source = sourceDevices;
            Device = device;
        }

        public Device Device { get; private set; }

		public string PresentationZone
		{
			get { return FiresecManager.GetPresentationZone(Device); }
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
				ServiceFactory.Events.GetEvent<FindElementEvent>().Publish(Device.PlanElementUIDs[0]);
        }
    }
}