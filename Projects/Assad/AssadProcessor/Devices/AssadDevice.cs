using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecClient;
using System.Diagnostics;

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
//            List<Assad.DeviceTypeParam> param = new List<Assad.DeviceTypeParam>();
            
            if (FiresecManager.CurrentStates.DeviceStates.Any(x => x.Id == Id))
            {
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == Id);

                Assad.DeviceTypeState mainState = new Assad.DeviceTypeState();
                mainState.state = "Состояние";
                mainState.value = deviceState.State.ToString();
                states.Add(mainState);
                string str = " ";
                switch (mainState.value)
                {
                    case "Тревога":
                                    str = mainState.value; break;
                    case "Внимание (предтревожное)": 
                                    str = mainState.value; break;
                    case "Неисправность": 
                                    str = mainState.value; break;
                    case "Требуется обслуживание": 
                                    str = mainState.value; break;
                    case "Норма(*)": 
                                    str = mainState.value; break;
                    default: break;
                }

                if (str != " ")
                {
                    FireEvent(str);
                }



                foreach (Parameter parameter in deviceState.Parameters)
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
//-->>            
                if (FiresecManager.CurrentConfiguration.AllDevices.Any(x => x.Id == Id))
                {
                    Device device = FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.Id == Id);
                    var driver = FiresecManager.CurrentConfiguration.Metadata.drv.FirstOrDefault(x=>x.id == device.DriverId);
                    
                    ////отладочная информация 
                    //{
                    //    string str0 = "GetState" + " для  " + DriversHelper.GetDriverNameById(device.DriverId) + " ---";
                    //    Trace.WriteLine(str0);                    
                    //}
                    Assad.DeviceTypeState state0 = new Assad.DeviceTypeState();
                    state0.state = "Примечание";
                    if ((string.IsNullOrEmpty(device.Description)) || (device.Description == "<NULL>"))
                    {
                        device.Description = " ";
                    }
                    state0.value = device.Description;
                    states.Add(state0);
                    ////отладочная информация 
                    //{
                    //    string str1 = "Состояние " + "Примечание:" + " - " + state0.value + " - ";
                    //    Trace.WriteLine(str1);
                    //}

                    if ((driver.minZoneCardinality == "1") && (driver.maxZoneCardinality == "1"))
                    {
                        Assad.DeviceTypeState state1 = new Assad.DeviceTypeState();
                        state1.state = "Зона";

                        if ((string.IsNullOrEmpty(device.ZoneNo)) || (device.ZoneNo == "<NULL>"))
                        {
                            device.ZoneNo = " ";
                        }
                        state1.value = device.ZoneNo;
                        states.Add(state1);

                        ////отладочная информация 
                        //{
                        //    string str2 = "Состояние " + "Зона:" + " - " + state1.value + " - ";
                        //    Trace.WriteLine(str2);
                        //}
                    }
                    else
                    {

                        if ((driver.options != null) && (driver.options.Contains("ExtendedZoneLogic")))
                        {
                            Assad.DeviceTypeState state2 = new Assad.DeviceTypeState();
                            state2.state = "Настройка включения по состоянию зон";
                            string zonelogicstring = ZoneLogicToText.Convert(device.ZoneLogic);
                            state2.value = zonelogicstring;
                            states.Add(state2);
                            ////отладочная информация 
                            //{
                            //    string str3 = "Состояния " + "Настройка включения по состоянию зон:" + " - " + state2.value + " - ";
                            //    Trace.WriteLine(str3);
                            //}
                        }
                        
                    }
                    if (driver.propInfo != null)
                    {
                        foreach (Firesec.Metadata.propInfoType propinfo in driver.propInfo)
                        {
                            Assad.DeviceTypeState loopState = new Assad.DeviceTypeState();
                            string name = propinfo.name;
                            string value =  propinfo.@default;                            
                            loopState.state = propinfo.caption;

                            if (propinfo.caption == "Адрес")
                            {
                                loopState.state = "Адрес USB устройства в сети RS-485";
                            }

                            if (device.Properties.Any(x => x.Name == name))
                            { // свойство присутствует
                                Property property = device.Properties.FirstOrDefault(x => x.Name == name);
                                value = property.Value;

                                if (string.IsNullOrEmpty(property.Value))
                                {
                                    value = propinfo.@default;
                                }
                            }


                            if (propinfo.param != null)
                            {// выбор значения из массива
                                if (propinfo.param.Any(x => x.value == value))
                                {
                                    value = propinfo.param.FirstOrDefault(x => x.value == value).name;
                                }
                            }
                                    
                            loopState.value = value;
                            ////отладочная информация 
                            //{
                            //    string loopstr = "Property " + " - " + loopState.state + " - " + " значение:" + loopState.value;
                            //    Trace.WriteLine(loopstr);
                            //}
                            if((propinfo.hidden == "0") && (propinfo.showOnlyInState == "0"))
                            states.Add(loopState);
                        }    

                      }
                    }//



//<<--            
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

            if (FiresecManager.CurrentStates.DeviceStates.Any(x => x.Id == Id))
            {
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == Id);
                List<Assad.CPeventTypeState> states = new List<Assad.CPeventTypeState>();

                Assad.CPeventTypeState mainState = new Assad.CPeventTypeState();
                mainState.state = "Состояние";
                mainState.value = deviceState.State.ToString();
                states.Add(mainState);

                //eventType.state[0] = new Assad.CPeventTypeState();
                //eventType.state[0].state = "Состояние"; 
                //eventType.state[0].value = deviceState.State;


                foreach (Parameter parameter in deviceState.Parameters)
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

            if (FiresecManager.CurrentStates.DeviceStates.Any(x => x.Id == Id))
            {
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Id == Id);

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
                Id = (Parent as AssadDevice).Id + @"/" + currentPath;
            }
            else
            {
                Id = currentPath;
            }
        }
    }
}
