using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Client
{
    public class Device : TreeBase
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string DeviceName { get; set; }
        public CommonState State { get; set; }
        public List<x.MHconfigTypeDeviceParam> Parameters { get; set; }
        public List<x.modelInfoTypeEvent> DeviceEvents { get; set; }
        public List<string> Commands { get; set; }

        x.MHconfigTypeDevice innerDevice;
        public x.MHconfigTypeDevice InnerDevice
        {
            get { return innerDevice; }
            set
            {
                innerDevice = value;

                try
                {
                    Address = innerDevice.param.First(x => x.param == "Адрес").value;
                }
                catch
                {
                    Address = null;
                }

                DeviceId = innerDevice.deviceId;
                DeviceType = innerDevice.type;
                DeviceName = innerDevice.deviceName;
                Parameters = new List<x.MHconfigTypeDeviceParam>(innerDevice.param);

                InnerType = DeviceTypesManager.GetModelInfo(DeviceType);

                Commands = new List<string>();
            }
        }

        x.modelInfoType innerType;
        public x.modelInfoType InnerType
        {
            get { return innerType; }
            set
            {
                innerType = value;
                State = new CommonState();
                if (innerType.@event != null)
                DeviceEvents = new List<x.modelInfoTypeEvent>(innerType.@event);
            }
        }

        public x.DeviceType GetInnerStates()
        {
            x.DeviceType deviceType = new x.DeviceType();
            deviceType.deviceId = innerDevice.deviceId;


            deviceType.state = new x.DeviceTypeState[1];
            deviceType.state[0] = new x.DeviceTypeState();
            deviceType.state[0].state = State.Name;
            deviceType.state[0].value = State.CurrentState;

            return deviceType;
        }

        public void ExecuteCommand(x.MHdeviceControlType controlType, string refMessageId)
        {
            Commands.Add(controlType.cmdId);
        }

        public void FireEvent(string eventName)
        {
            x.CPeventType eventType = new x.CPeventType();

            eventType.deviceId = InnerDevice.deviceId;
            eventType.eventTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            eventType.eventId = eventName;
            eventType.alertLevel = "0";

            eventType.state = new x.CPeventTypeState[1];
            eventType.state[0] = new x.CPeventTypeState();
            eventType.state[0].state = State.Name;
            eventType.state[0].value = State.CurrentState;

            NetManager.Send(eventType, null);
        }
    }
}
