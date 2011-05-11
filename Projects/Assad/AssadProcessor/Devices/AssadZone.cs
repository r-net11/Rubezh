using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;

namespace AssadProcessor.Devices
{
    public class AssadZone : AssadBase
    {
        public string ZoneNo { get; set; }

        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            if (innerDevice.param != null);
            if (innerDevice.param.Any(x => x.param == "Номер зоны"))
                ZoneNo = innerDevice.param.FirstOrDefault(x => x.param == "Адрес").value;
        }

        public override Assad.DeviceType GetStates()
        {
            Assad.DeviceType deviceType = new Assad.DeviceType();
            deviceType.deviceId = DeviceId;
            List<Assad.DeviceTypeState> states = new List<Assad.DeviceTypeState>();

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

        public override void FireEvent(string eventName)
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

            NetManager.Send(eventType, null);
        }

        public override Assad.DeviceType QueryAbility()
        {
            Assad.DeviceType deviceAbility = new Assad.DeviceType();
            deviceAbility.deviceId = DeviceId;
            List<Assad.DeviceTypeParam> abilityParameters = new List<Assad.DeviceTypeParam>();

            deviceAbility.param = abilityParameters.ToArray();
            return deviceAbility;
        }
    }
}
