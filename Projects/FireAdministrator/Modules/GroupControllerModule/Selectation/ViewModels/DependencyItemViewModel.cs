using RubezhAPI.GK;
using GKModule.Events;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Events;

namespace GKModule.ViewModels
{
	public class DependencyItemViewModel
	{
		public string Name { get; set; }
		public string ImageSource { get; set; }

		GKBase gkBase;

		public DependencyItemViewModel(GKBase gkBase)
		{
			ShowCommand =  new RelayCommand(OnShow);

			this.gkBase = gkBase;
			Name = gkBase.PresentationName;
			ImageSource = gkBase.ImageSource;
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			if (gkBase is GKDoor)
				ServiceFactoryBase.Events.GetEvent<ShowGKDoorEvent>().Publish(gkBase.UID);
			if (gkBase is GKDirection)
				ServiceFactoryBase.Events.GetEvent<ShowGKDirectionEvent>().Publish(gkBase.UID);
			if (gkBase is GKDelay)
				ServiceFactoryBase.Events.GetEvent<ShowXDelayEvent>().Publish(gkBase.UID);
			if (gkBase is GKMPT)
				ServiceFactoryBase.Events.GetEvent<ShowGKMPTEvent>().Publish(gkBase.UID);
			if (gkBase is GKPumpStation)
			{
				ServiceFactoryBase.Events.GetEvent<ShowGKPumpStationEvent>().Publish(gkBase.UID);
				ServiceFactoryBase.Events.GetEvent<ShowGKPumpStationOnPlanEvent>().Publish(gkBase.UID);
			}
			if (gkBase is GKDevice)
				ServiceFactoryBase.Events.GetEvent<ShowGKDeviceEvent>().Publish(gkBase.UID);
			if (gkBase is GKGuardZone)
				ServiceFactoryBase.Events.GetEvent<ShowGKGuardZoneEvent>().Publish(gkBase.UID);
			if (gkBase is GKZone)
				ServiceFactoryBase.Events.GetEvent<ShowGKZoneEvent>().Publish(gkBase.UID);
		}
	}
}
