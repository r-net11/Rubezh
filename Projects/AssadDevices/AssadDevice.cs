using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace AssadDdevices
{
    public class AssadDevice : TreeBase
    {
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string DeviceName { get; set; }

        AssadState state;
        public AssadState State
        {
            get {return state;}
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }
        
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
                    string stringAddress = innerDevice.param.First(x => x.param == "Адрес").value;
                    int intAddress = Convert.ToInt32(stringAddress);

                    x.MHconfigTypeDeviceParam shleifParam = innerDevice.param.FirstOrDefault(x => x.param == "Номер шлейфа (АЛС)");
                    if (shleifParam != null)
                    {
                        int intShleif = Convert.ToInt32(shleifParam.value);
                        intAddress = intShleif * 256 + intAddress;
                    }

                    Address = intAddress.ToString();
                }
                catch
                {
                    Address = null;
                }

                DeviceId = innerDevice.deviceId;
                DeviceType = innerDevice.type;
                DeviceName = innerDevice.deviceName;
                Parameters = new List<x.MHconfigTypeDeviceParam>(innerDevice.param);

                InnerType = AssadDeviceTypesManager.GetModelInfo(DeviceType);

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
                State = new AssadState();
                if (innerType != null)
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
            deviceType.state[0].value = State.State;

            return deviceType;
        }

        public void ExecuteCommand(x.MHdeviceControlType controlType, string refMessageId)
        {
            Commands.Add(controlType.cmdId);
        }

        public x.CPeventType CreateEvent(string eventName)
        {
            x.CPeventType eventType = new x.CPeventType();

            eventType.deviceId = InnerDevice.deviceId;
            eventType.eventTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            eventType.eventId = eventName;
            eventType.alertLevel = "0";

            eventType.state = new x.CPeventTypeState[1];
            eventType.state[0] = new x.CPeventTypeState();
            eventType.state[0].state = State.Name;
            eventType.state[0].value = State.State;

            return eventType;
        }
    }
}
