using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using ClientApi;

namespace AssadDevices
{
    public class AssadBase
    {
        public AssadBase()
        {
            Children = new List<AssadBase>();
            Properties = new List<AssadProperty>();
        }

        public AssadBase Parent { get; set; }
        public List<AssadBase> Children { get; set; }

        public string DeviceId { get; set; }
        public string DriverId { get; set; }
        public string Address { get; set; }
        public string Path { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }
        public List<AssadProperty> Properties { get; set; }
        public string Zone { get; set; }
        public string ZoneLogic { get; set; }
        public string ValidationError { get; set; }

        // это свойство задается при конфигурации из ассада
        // нужно сделать это свойство методом
        // ведется разграничение какое это устройство - зона или устройство

        public virtual void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            DriverId = AssadDeviceFactory.GetDriverId(innerDevice);
            DeviceId = innerDevice.deviceId;
            DeviceName = innerDevice.deviceName;

            // сброс валидации
            ValidationError = "";

            // тупо скопировать все свойства от конфигурации из ассада и чтобы потом перенести их в конфигурацию сервиса
            // адрес и причие значения конфигурации слудует в дальнейшем дергать из этого списка
            Properties = new List<AssadProperty>();
            if (innerDevice.param != null)
            {
                if (innerDevice.param.Length > 0)
                {
                    foreach (Assad.MHconfigTypeDeviceParam assadParam in innerDevice.param)
                    {
                        AssadProperty assadProperty = new AssadProperty();
                        assadProperty.Name = assadParam.param;
                        assadProperty.Value = assadParam.value;
                        Properties.Add(assadProperty);
                    }
                }
            }

            Description = Properties.First(x => x.Name == "Примечание").Value;
            Properties.Remove(Properties.First(x => x.Name == "Примечание"));
        }

        public void SetPath()
        {
            string currentPath = DriverId + ":" + Address;
            if (Parent != null)
            {
                Path = (Parent as AssadBase).Path + @"/" + currentPath;
            }
            else
            {
                Path = currentPath;
            }
        }

        // установить валидационную ошибку
        public void SetValidationError(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                ValidationError = error;
                AssadConfiguration.IsValid = false;
                AssadConfiguration.InvalidDevices.Add(this);
            }
        }

        // передать в ассад информацию о  текушем состоянии устройства

        public virtual Assad.DeviceType GetInnerStates()
        {
            Assad.DeviceType deviceType = new Assad.DeviceType();
            deviceType.deviceId = DeviceId;
            List<Assad.DeviceTypeState> states = new List<Assad.DeviceTypeState>();

            Assad.DeviceTypeState configurationState = new Assad.DeviceTypeState();
            configurationState.state = "Конфигурация";
            if (string.IsNullOrEmpty(ValidationError))
                configurationState.value = "Норма";
            else
                configurationState.value = ValidationError;
            states.Add(configurationState);

            if (ServiceClient.CurrentStates.DeviceStates.Any(x => x.Path == Path))
            {
                DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == Path);

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

        // свормировать событие с устройством и послать его ассаду

        public virtual Assad.CPeventType CreateEvent(string eventName)
        {
            Assad.CPeventType eventType = new Assad.CPeventType();

            eventType.deviceId = DeviceId;
            eventType.eventTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            eventType.eventId = eventName;
            eventType.alertLevel = "0";

            if (ServiceClient.CurrentStates.DeviceStates.Any(x => x.Path == Path))
            {
                DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == Path);

                eventType.state = new Assad.CPeventTypeState[1];
                eventType.state[0] = new Assad.CPeventTypeState();
                eventType.state[0].state = "Состояние";
                eventType.state[0].value = deviceState.State;
            }

            return eventType;
        }

        // передать в ассад информацию о дополнительных параметрах устройства
        // а также информацию о валидности концигурации

        public virtual Assad.DeviceType QueryAbility()
        {
            Assad.DeviceType deviceAbility = new Assad.DeviceType();
            deviceAbility.deviceId = DeviceId;
            List<Assad.DeviceTypeParam> abilityParameters = new List<Assad.DeviceTypeParam>();

            Assad.DeviceTypeParam configurationParameter = new Assad.DeviceTypeParam();
            abilityParameters.Add(configurationParameter);
            configurationParameter.name = "Конфигурация";
            if (string.IsNullOrEmpty(ValidationError))
                configurationParameter.value = "Норма";
            else
                configurationParameter.value = ValidationError;

            if (ServiceClient.CurrentStates.DeviceStates.Any(x => x.Path == Path))
            {
                DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == Path);
                
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

        List<AssadBase> allChildren;
        public List<AssadBase> FindAllChildren()
        {
            allChildren = new List<AssadBase>();
            allChildren.Add(this);
            FindChildren(this);
            return allChildren;
        }

        void FindChildren(AssadBase parent)
        {
            if (parent.Children != null)
                foreach (AssadBase child in parent.Children)
                {
                    allChildren.Add(child);
                    FindChildren(child);
                }
        }
    }
}
