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
            if (innerDevice.param != null)
            {
                var zoneParameter = innerDevice.param.FirstOrDefault(x => x.param == "Номер зоны");
                if (zoneParameter != null)
                    ZoneNo = zoneParameter.value;
            }
        }

        public override Assad.DeviceType GetStates()
        {
            Assad.DeviceType deviceType = new Assad.DeviceType();
            deviceType.deviceId = DeviceId;
            List<Assad.DeviceTypeState> states = new List<Assad.DeviceTypeState>();

            if (FiresecManager.DeviceStates.ZoneStates.Any(x => x.No == ZoneNo))
            {
                var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == ZoneNo);
                var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZoneNo);

                Assad.DeviceTypeState mainState = new Assad.DeviceTypeState();
                mainState.state = "Состояние";
                mainState.value = zoneState.State.ToString();
                states.Add(mainState);
                Assad.DeviceTypeState state1 = new Assad.DeviceTypeState();
                state1.state = "Наименование";
                state1.value = zone.Name;
                states.Add(state1);
                Assad.DeviceTypeState state2 = new Assad.DeviceTypeState();
                state2.state = "Число датчиков для формирования сигнала Пожар";
                state2.value = zone.DetectorCount;
                states.Add(state2);
                Assad.DeviceTypeState state3 = new Assad.DeviceTypeState();
                state3.state = "Время эвакуации";
                state3.value = zone.EvacuationTime;
                states.Add(state3);
                Assad.DeviceTypeState state4 = new Assad.DeviceTypeState();
                state4.state = "Примечание";
                state4.value = zone.Description;
                states.Add(state4);
                Assad.DeviceTypeState state5 = new Assad.DeviceTypeState();
                state5.state = "Назначение зоны";
                state5.value = "Пожарная";
                states.Add(state5);
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

            if (FiresecManager.DeviceStates.ZoneStates.Any(x => x.No == ZoneNo))
            {
                var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == ZoneNo);

                eventType.state = new Assad.CPeventTypeState[1];
                eventType.state[0] = new Assad.CPeventTypeState();
                eventType.state[0].state = "Состояние";
                eventType.state[0].value = zoneState.State.ToString();
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
