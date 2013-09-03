using System.Collections.Generic;
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