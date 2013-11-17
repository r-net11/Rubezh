using System.Collections.Generic;
using Infrastructure;
using XFiresecAPI;
using FiresecClient;

namespace GKModule.ViewModels
{
    public class DirectionDeviceViewModel
    {
        public DirectionDeviceViewModel(XDirectionDevice directionDevice)
        {
            DirectionDevice = directionDevice;
        }

        public XDirectionDevice DirectionDevice { get; private set; }

		public string PresentationZone
		{
			get
			{
				if (DirectionDevice.Device.IsNotUsed)
					return null;
				return XManager.GetPresentationZone(DirectionDevice.Device);
			}
		}

        public XStateBit StateType
        {
            get { return DirectionDevice.StateBit; }
            set
            {
				DirectionDevice.StateBit = value;
                ServiceFactory.SaveService.GKChanged = true;
            }
        }

		public List<XStateBit> StateTypes
        {
            get { return DirectionDevice.Device.Driver.AvailableStateBits; }
        }
    }
}