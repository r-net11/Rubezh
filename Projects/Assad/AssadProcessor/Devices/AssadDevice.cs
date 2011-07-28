using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using FiresecAPI.Models;

namespace AssadProcessor.Devices
{
    public class AssadDevice : AssadBase
    {
        public string Address { get; set; }
        public string Id { get; set; }
        public string DriverId { get; set; }

        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            if (innerDevice.param == null)
                return;

            var addressParameter = innerDevice.param.FirstOrDefault(x => x.param == "Адрес");
            if (addressParameter != null)
                Address = addressParameter.value;
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
//            List<Assad.DeviceTypeParam> param = new List<Assad.DeviceTypeParam>();

            if (FiresecManager.States.DeviceStates.Any(x => x.Id == Id))
            {
                var deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == Id);

                Assad.DeviceTypeState mainState = new Assad.DeviceTypeState();
                mainState.state = "Состояние";
                mainState.value = deviceState.State.ToString();
                states.Add(mainState);
                string str = " ";
                switch (mainState.value)
                {
                    case "Тревога":
                    case "Внимание (предтревожное)":
                    case "Неисправность":
                    case "Требуется обслуживание":
                    case "Норма(*)":
                        str = mainState.value;
                        break;
                }

                if (str != " ")
                {
                    FireEvent(str);
                }

                foreach (var parameter in deviceState.Parameters)
                {
                    if (parameter.Visible)
                    {
                        Assad.DeviceTypeState parameterState = new Assad.DeviceTypeState();
                        parameterState.state = parameter.Caption;
                        parameterState.value = parameter.Value;

                        if ((string.IsNullOrEmpty(parameter.Value)) || (parameter.Value == "<NULL>"))
                        {
                            parameterState.value = " ";
                        }

                        states.Add(parameterState);
                    }
                }
 
                if (FiresecManager.Configuration.Devices.Any(x => x.Id == Id))
                {
                    var device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.Id == Id);

                    Assad.DeviceTypeState state0 = new Assad.DeviceTypeState();
                    state0.state = "Примечание";
                    if ((string.IsNullOrEmpty(device.Description)) || (device.Description == "<NULL>"))
                    {
                        device.Description = " ";
                    }
                    state0.value = device.Description;
                    states.Add(state0);

                    if (device.Driver.IsZoneDevice)
                    {
                        Assad.DeviceTypeState state1 = new Assad.DeviceTypeState();
                        state1.state = "Зона";

                        if ((string.IsNullOrEmpty(device.ZoneNo)) || (device.ZoneNo == "<NULL>"))
                        {
                            device.ZoneNo = " ";
                        }
                        state1.value = device.ZoneNo;
                        states.Add(state1);
                    }
                    else
                    {

                        if (device.Driver.IsZoneLogicDevice)
                        {
                            Assad.DeviceTypeState state2 = new Assad.DeviceTypeState();
                            state2.state = "Настройка включения по состоянию зон";
                            string zonelogicstring = device.ZoneLogic.ToString();
                            state2.value = zonelogicstring;
                            states.Add(state2);
                        }

                    }
                    foreach (var propinfo in device.Driver.Properties)
                    {
                        Assad.DeviceTypeState loopState = new Assad.DeviceTypeState();
                        string name = propinfo.Name;
                        string value = propinfo.Default;
                        loopState.state = propinfo.Caption;

                        if (propinfo.Caption == "Адрес")
                        {
                            loopState.state = "Адрес USB устройства в сети RS-485";
                        }

                        if (device.Properties.Any(x => x.Name == name))
                        {
                            Property property = device.Properties.FirstOrDefault(x => x.Name == name);
                            value = property.Value;

                            if (string.IsNullOrEmpty(property.Value))
                            {
                                value = propinfo.Default;
                            }
                        }

                        var parameter = propinfo.Parameters.FirstOrDefault(x => x.Value == value);
                        if (parameter != null)
                        {
                            value = parameter.Name;
                        }

                        loopState.value = value;

                        if (propinfo.Visible)
                            states.Add(loopState);
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

            if (FiresecManager.States.DeviceStates.Any(x => x.Id == Id))
            {
                var deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == Id);
                List<Assad.CPeventTypeState> states = new List<Assad.CPeventTypeState>();

                Assad.CPeventTypeState mainState = new Assad.CPeventTypeState();
                mainState.state = "Состояние";
                mainState.value = deviceState.State.ToString();
                states.Add(mainState);

                foreach (var parameter in deviceState.Parameters)
                {
                    if (parameter.Visible)
                    {
                        Assad.CPeventTypeState parameterState = new Assad.CPeventTypeState();
                        parameterState.state = parameter.Name;
                        parameterState.value = parameter.Value;

                        if ((string.IsNullOrEmpty(parameter.Value)) || (parameter.Value == "<NULL>"))
                        {
                            parameterState.value = " ";
                        }
                        states.Add(parameterState);
                    }
                }                
                eventType.state = states.ToArray();
            }
            NetManager.Send(eventType, null);
        }

        public override Assad.DeviceType QueryAbility()
        {
            Assad.DeviceType deviceAbility = new Assad.DeviceType();
            deviceAbility.deviceId = DeviceId;
            List<Assad.DeviceTypeParam> abilityParameters = new List<Assad.DeviceTypeParam>();

            if (FiresecManager.States.DeviceStates.Any(x => x.Id == Id))
            {
                var deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == Id);

                foreach (var parameter in deviceState.Parameters)
                {
                    if (!(string.IsNullOrEmpty(parameter.Value)) && (parameter.Value != "<NULL>"))
                    {
                        Assad.DeviceTypeParam abilityParameter = new Assad.DeviceTypeParam();
                        abilityParameter.name = parameter.Caption;
                        abilityParameter.value = parameter.Value;
                        abilityParameters.Add(abilityParameter);
                    }
                }

                foreach (var state in deviceState.States)
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
                Id = (Parent as AssadDevice).Id + @"/" + currentPath;
            }
            else
            {
                Id = currentPath;
            }
        }
    }
}
