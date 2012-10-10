using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using Infrastructure;

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
                ServiceFactory.SaveService.XDevicesChanged = true;
            }
        }

        public List<XStateType> StateTypes
        {
            get { return DirectionDevice.Device.Driver.AvailableStates; }
        }
    }
}