using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;

namespace AssadDevices
{
    public class AssadZone : AssadBase
    {
        public string ZoneNo { get; set; }
        public string ZoneName { get; set; }
        public string DetectorCount { get; set; }
        public string EvecuationTime { get; set; }

        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            base.SetInnerDevice(innerDevice);

            try
            {
                ZoneNo = Properties.First(x => x.Name == "Номер зоны").Value;
                ZoneName = Properties.First(x => x.Name == "Наименование").Value;
                DetectorCount = Properties.First(x => x.Name == "Число датчиков для формирования сигнала Пожар").Value;
                EvecuationTime = Properties.First(x => x.Name == "Время эвакуации").Value;
            }
            catch
            {
                throw new Exception("Неправильный формат зоны при конфигурации из ассада");
            }
        }

        public override Assad.DeviceType GetInnerStates()
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

            if (FiresecManager.CurrentStates.ZoneStates.Any(x => x.No == ZoneNo))
            {
                ZoneState zoneState = FiresecManager.CurrentStates.ZoneStates.FirstOrDefault(x => x.No == ZoneNo);

                Assad.DeviceTypeState mainState = new Assad.DeviceTypeState();
                mainState.state = "Состояние";
                mainState.value = zoneState.State;
                states.Add(mainState);
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

        public override Assad.CPeventType CreateEvent(string eventName)
        {
            Assad.CPeventType eventType = new Assad.CPeventType();

            eventType.deviceId = DeviceId;
            eventType.eventTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            eventType.eventId = eventName;
            eventType.alertLevel = "0";

            if (FiresecManager.CurrentStates.ZoneStates.Any(x => x.No == ZoneNo))
            {
                ZoneState zoneState = FiresecManager.CurrentStates.ZoneStates.FirstOrDefault(x => x.No == ZoneNo);

                eventType.state = new Assad.CPeventTypeState[1];
                eventType.state[0] = new Assad.CPeventTypeState();
                eventType.state[0].state = "Состояние";
                eventType.state[0].value = zoneState.State;
            }

            return eventType;
        }

        public override Assad.DeviceType QueryAbility()
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

            deviceAbility.param = abilityParameters.ToArray();
            return deviceAbility;
        }
    }
}
