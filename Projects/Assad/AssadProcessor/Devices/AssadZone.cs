using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
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
            var deviceType = new Assad.DeviceType();
            deviceType.deviceId = DeviceId;
            var states = new List<Assad.DeviceTypeState>();

            if (FiresecManager.DeviceStates.ZoneStates.Any(x => x.No == ZoneNo))
            {
                var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == ZoneNo);
                var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZoneNo);

                var mainState = new Assad.DeviceTypeState()
                {
                    state = "Состояние",
                    value = EnumsConverter.StateTypeToClassName(zoneState.StateType)
                };
                states.Add(mainState);
                var state1 = new Assad.DeviceTypeState()
                {
                    state = "Наименование",
                    value = zone.Name
                };
                states.Add(state1);
                var state2 = new Assad.DeviceTypeState()
                {
                    state = "Число датчиков для формирования сигнала Пожар",
                    value = zone.DetectorCount
                };
                states.Add(state2);
                var state3 = new Assad.DeviceTypeState()
                {
                    state = "Время эвакуации",
                    value = zone.EvacuationTime
                };
                states.Add(state3);
                var state4 = new Assad.DeviceTypeState()
                {
                    state = "Примечание",
                    value = zone.Description
                };
                states.Add(state4);
                var state5 = new Assad.DeviceTypeState()
                {
                    state = "Назначение зоны",
                    value = "Пожарная"
                };
                states.Add(state5);
            }
            else
            {
                var mainState = new Assad.DeviceTypeState()
                {
                    state = "Состояние",
                    value = "Отсутствует в конфигурации сервера оборудования"
                };
                states.Add(mainState);
            }

            deviceType.state = states.ToArray();
            return deviceType;
        }

        public override void FireEvent(string eventName)
        {
            var eventType = new Assad.CPeventType();

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
                eventType.state[0].value = EnumsConverter.StateTypeToClassName(zoneState.StateType);
            }

            NetManager.Send(eventType, null);
        }

        public override Assad.DeviceType QueryAbility()
        {
            var deviceAbility = new Assad.DeviceType();
            deviceAbility.deviceId = DeviceId;
            var abilityParameters = new List<Assad.DeviceTypeParam>();

            deviceAbility.param = abilityParameters.ToArray();
            return deviceAbility;
        }
    }
}