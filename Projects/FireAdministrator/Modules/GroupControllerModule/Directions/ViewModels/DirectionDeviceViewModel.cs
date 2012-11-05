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

        public XStateType StateType
        {
            get { return DirectionDevice.StateType; }
            set
            {
                DirectionDevice.StateType = value;
                ServiceFactory.SaveService.GKChanged = true;
            }
        }

        public List<XStateType> StateTypes
        {
            get { return DirectionDevice.Device.Driver.AvailableStates; }
        }
    }
}