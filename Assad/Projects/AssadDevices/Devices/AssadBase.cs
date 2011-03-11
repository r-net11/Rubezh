using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace AssadDevices
{
    public class AssadBase : AssadTreeBase
    {
        public string DeviceId { get; set; }
        public string DriverId { get; set; }
        public string Address { get; set; }
        public string Path { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }
        public AssadState State { get; set; }
        public string SourceState { get; set; }
        public List<AssadProperty> Properties { get; set; }
        public Assad.modelInfoType InnerType { get; set; }
        public List<string> Zones { get; set; }
        public string ValidationError { get; set; }

        // это свойство задается при конфигурации из ассада
        // нужно сделать это свойство методом
        // ведется разграничение какое это устройство - зона или устройство

        public virtual void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            DriverId = AssadDeviceFactory.GetDriverId(innerDevice);
            DeviceId = innerDevice.deviceId;
            DeviceName = innerDevice.deviceName;
            InnerType = AssadServices.AssadDeviceTypesManager.GetModelInfo(innerDevice.type);
            State = new AssadState();

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

        public List<string> ExtractZones(string zones)
        {
            try
            {
                string[] separators = new string[1];
                separators[0] = ";";
                string[] separatedZones = zones.Split(separators, StringSplitOptions.None);
                return separatedZones.ToList();
            }
            catch
            {
                Trace.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxx");
                return null;
            }
        }

        // передать в ассад информацию о  текушем состоянии устройства

        public virtual Assad.DeviceType GetInnerStates()
        {
            Assad.DeviceType deviceType = new Assad.DeviceType();
            deviceType.deviceId = DeviceId;

            deviceType.state = new Assad.DeviceTypeState[3];
            deviceType.state[0] = new Assad.DeviceTypeState();
            deviceType.state[0].state = State.Name;
            deviceType.state[0].value = State.State;

            deviceType.state[1] = new Assad.DeviceTypeState();
            deviceType.state[1].state = "Состояние дополнительно";
            if (string.IsNullOrEmpty(SourceState))
                deviceType.state[1].value = " ";
            else
                deviceType.state[1].value = SourceState;

            deviceType.state[2] = new Assad.DeviceTypeState();
            deviceType.state[2].state = "Конфигурация";
            if (string.IsNullOrEmpty(ValidationError))
                deviceType.state[2].value = " ";
            else
                deviceType.state[2].value = ValidationError;

            return deviceType;
        }

        // свормировать событие с устройством и послать его ассаду

        public Assad.CPeventType CreateEvent(string eventName)
        {
            Assad.CPeventType eventType = new Assad.CPeventType();

            eventType.deviceId = DeviceId;
            eventType.eventTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            eventType.eventId = eventName;
            eventType.alertLevel = "0";

            eventType.state = new Assad.CPeventTypeState[1];
            eventType.state[0] = new Assad.CPeventTypeState();
            eventType.state[0].state = State.Name;
            eventType.state[0].value = State.State;

            return eventType;
        }

        // передать в ассад информацию о дополнительных параметрах устройства
        // а также информацию о валидности концигурации

        public Assad.DeviceType GetParameters()
        {
            Assad.DeviceType deviceParameters = new Assad.DeviceType();
            deviceParameters.deviceId = DeviceId;
            deviceParameters.param = new Assad.DeviceTypeParam[1];
            deviceParameters.param[0] = new Assad.DeviceTypeParam();
            deviceParameters.param[0].name = "Конфигурация";
            if (string.IsNullOrEmpty(ValidationError))
                deviceParameters.param[0].value = "Норма";
            else
                deviceParameters.param[0].value = ValidationError;
            return deviceParameters;
        }
    }
}
