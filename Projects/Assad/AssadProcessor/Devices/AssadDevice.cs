using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecClient;

namespace AssadProcessor.Devices
{
    public class AssadDevice : AssadBase
    {
        public string Address { get; set; }
        public string Path { get; set; }
        public string DriverId { get; set; }

        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            if (innerDevice.param == null)
                return;

            if (innerDevice.param.Any(x => x.param == "Адрес"))
                Address = innerDevice.param.FirstOrDefault(x => x.param == "Адрес").value;
            else
                Address = null;

            string driverName = DriversHelper.GetDriverNameById(DriverId);
            switch (driverName)
            {
                case "Компьютер":
                case "Насосная Станция":
                case "Жокей-насос":
                case "Компрессор":
                case "Дренажный насос":
                case "Насос компенсации утечек":
                    Address = "0";
                    break;

                case "USB преобразователь МС-1":
                case "USB преобразователь МС-2":
                    {
                        string serialNo = null;
                        if (innerDevice.param.Any(x => x.param == "Серийный номер"))
                            serialNo = innerDevice.param.FirstOrDefault(x => x.param == "Серийный номер").value;

                        if (string.IsNullOrEmpty(serialNo))
                        {
                            Address = "0";
                        }
                        else
                        {
                            Address = serialNo;
                        }
                    }
                    break;
            }

            SetPath();
        }

        public override Assad.DeviceType GetStates()
        {
            Assad.DeviceType deviceType = new Assad.DeviceType();
            deviceType.deviceId = DeviceId;
            List<Assad.DeviceTypeState> states = new List<Assad.DeviceTypeState>();

            if (FiresecManager.CurrentStates.DeviceStates.Any(x => x.Path == Path))
            {
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == Path);

                Assad.DeviceTypeState mainState = new Assad.DeviceTypeState();
                mainState.state = "Состояние";
                mainState.value = deviceState.State;
                states.Add(mainState);

                foreach (Parameter parameter in deviceState.Parameters)
                {
                    if (parameter.Visible)
                    {
                        Assad.DeviceTypeState parameterState = new Assad.DeviceTypeState();
                        parameterState.state = parameter.Name;
                        parameterState.value = parameter.Value;

                        if (!(string.IsNullOrEmpty(parameter.Value)) && (parameter.Value != "<NULL>"))
                        {
                            parameterState.value = " ";
                        }

                        states.Add(parameterState);
                    }
                }
            }
            else
            {
                Assad.DeviceTypeState mainState = new Assad.DeviceTypeState();
                mainState.state = "Состояние";
                mainState.value = "Отсутствует в конфигурации сервера оборудования";
                states.Add(mainState);
            }

            deviceType.state = states.ToArray();
            return deviceType;
        }

        public override void FireEvent(string eventName)
        {
            Assad.CPeventType eventType = new Assad.CPeventType();

            eventType.deviceId = DeviceId;
            eventType.eventTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            eventType.eventId = eventName;
            eventType.alertLevel = "0";

            if (FiresecManager.CurrentStates.DeviceStates.Any(x => x.Path == Path))
            {
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == Path);

                eventType.state = new Assad.CPeventTypeState[1];
                eventType.state[0] = new Assad.CPeventTypeState();
                eventType.state[0].state = "Состояние";
                eventType.state[0].value = deviceState.State;
            }

            NetManager.Send(eventType, null);
        }

        public override Assad.DeviceType QueryAbility()
        {
            Assad.DeviceType deviceAbility = new Assad.DeviceType();
            deviceAbility.deviceId = DeviceId;
            List<Assad.DeviceTypeParam> abilityParameters = new List<Assad.DeviceTypeParam>();

            if (FiresecManager.CurrentStates.DeviceStates.Any(x => x.Path == Path))
            {
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == Path);

                foreach (Parameter parameter in deviceState.Parameters)
                {
                    if (!(string.IsNullOrEmpty(parameter.Value)) && (parameter.Value != "<NULL>"))
                    {
                        Assad.DeviceTypeParam abilityParameter = new Assad.DeviceTypeParam();
                        abilityParameter.name = parameter.Caption;
                        abilityParameter.value = parameter.Value;
                        abilityParameters.Add(abilityParameter);
                    }
                }

                foreach (string state in deviceState.States)
                {
                    Assad.DeviceTypeParam stateParameter = new Assad.DeviceTypeParam();
                    stateParameter.name = state;
                    stateParameter.value = " ";
                    abilityParameters.Add(stateParameter);
                }
            }

            deviceAbility.param = abilityParameters.ToArray();
            return deviceAbility;
        }

        void SetPath()
        {
            string currentPath = DriverId + ":" + Address;
            if (Parent != null)
            {
                Path = (Parent as AssadDevice).Path + @"/" + currentPath;
            }
            else
            {
                Path = currentPath;
            }
        }
    }
}
