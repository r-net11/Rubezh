using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;

namespace GKModule.ViewModels
{
	public class DirectionDeviceViewModel
	{
		public DirectionDeviceViewModel(GKDirectionDevice directionDevice)
		{
			DirectionDevice = directionDevice;
		}

		public GKDirectionDevice DirectionDevice { get; private set; }

		public string PresentationZone
		{
			get
			{
				if (DirectionDevice.Device.IsNotUsed)
					return null;
				return GKManager.GetPresentationZone(DirectionDevice.Device);
			}
		}

		public GKStateBit StateType
		{
			get { return DirectionDevice.StateBit; }
			set
			{
				DirectionDevice.StateBit = value;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public List<GKStateBit> StateTypes
		{
			get { return DirectionDevice.Device.Driver.AvailableStateBits; }
		}
	}
}