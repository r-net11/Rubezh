using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientApi;
using FiresecMetadata;

namespace AssadDevices
{
    public class AssadMonitor : AssadBase
    {
        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
            base.SetInnerDevice(innerDevice);
        }

        public override Assad.DeviceType GetInnerStates()
        {
            Assad.DeviceType deviceType = new Assad.DeviceType();
            deviceType.deviceId = DeviceId;
            List<Assad.DeviceTypeState> states = new List<Assad.DeviceTypeState>();

            Assad.DeviceTypeState configurationState = new Assad.DeviceTypeState();
            configurationState.state = "Конфигурация";
            configurationState.value = "Норма";
            states.Add(configurationState);

            Assad.DeviceTypeState state1 = new Assad.DeviceTypeState();
            state1.state = "Тревога";
            state1.value = "Нет";
            states.Add(state1);

            Assad.DeviceTypeState state2 = new Assad.DeviceTypeState();
            state2.state = "Внимание (предтревожное)";
            state2.value = "Нет";
            states.Add(state2);

            Assad.DeviceTypeState state3 = new Assad.DeviceTypeState();
            state3.state = "Неисправность";
            state3.value = "Нет";
            states.Add(state3);

            Assad.DeviceTypeState state4 = new Assad.DeviceTypeState();
            state4.state = "Требуется обслуживание";
            state4.value = "Нет";
            states.Add(state4);

            Assad.DeviceTypeState state5 = new Assad.DeviceTypeState();
            state5.state = "Обход устройств";
            state5.value = "Нет";
            states.Add(state5);

            Assad.DeviceTypeState state6 = new Assad.DeviceTypeState();
            state6.state = "Неопределено";
            state6.value = "Нет";
            states.Add(state6);

            Assad.DeviceTypeState state7 = new Assad.DeviceTypeState();
            state7.state = "Норма(*)";
            state7.value = "Нет";
            states.Add(state7);

            Assad.DeviceTypeState state8 = new Assad.DeviceTypeState();
            state8.state = "Норма";
            state8.value = "Нет";
            states.Add(state8);

            deviceType.state = states.ToArray();
            return deviceType;
        }

        public virtual Assad.CPeventType CreateEvent(string eventName)
        {
            Assad.CPeventType eventType = new Assad.CPeventType();

            eventType.deviceId = DeviceId;
            eventType.eventTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            eventType.eventId = eventName;
            eventType.alertLevel = "0";

            List<StateTypeCounter> counters = new List<StateTypeCounter>();
            counters.Add(new StateTypeCounter() { StateType = StateType.Alarm });
            counters.Add(new StateTypeCounter() { StateType = StateType.Failure});
            counters.Add(new StateTypeCounter() { StateType = StateType.Info });
            //counters.Add(new StateTypeCounter() { StateType = StateType.No });
            counters.Add(new StateTypeCounter() { StateType = StateType.Norm });
            counters.Add(new StateTypeCounter() { StateType = StateType.Off });
            counters.Add(new StateTypeCounter() { StateType = StateType.Service });
            counters.Add(new StateTypeCounter() { StateType = StateType.Unknown });
            counters.Add(new StateTypeCounter() { StateType = StateType.Warning });

            foreach (DeviceState deviceState in ServiceClient.CurrentStates.DeviceStates)
            {
                StateType stateType = StateHelper.NameToType(deviceState.State);
                StateTypeCounter stateTypeCounter = counters.FirstOrDefault(x => x.StateType == stateType);
                if (stateTypeCounter != null)
                {
                    stateTypeCounter.Count++;
                }
            }

            List<Assad.CPeventTypeState> states = new List<Assad.CPeventTypeState>();

            foreach (StateTypeCounter stateTypeCounter in counters)
            {
                Assad.CPeventTypeState AlarmState = new Assad.CPeventTypeState();
                AlarmState.state = StateHelper.TypeToName(stateTypeCounter.StateType);
                AlarmState.value = (stateTypeCounter.Count > 0) ? "Есть" : "Нет";
                states.Add(AlarmState);
            }

            eventType.state = states.ToArray();

            return eventType;
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

    class StateTypeCounter
    {
        public StateType StateType { get; set; }
        public int Count { get; set; }
    }
}
