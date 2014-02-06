using System.Collections.Generic;
using FiresecClient;
using Infrastructure;
using XFiresecAPI;

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