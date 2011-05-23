using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecClient;

namespace AssadProcessor.Devices
{
    public class AssadMonitor : AssadBase
    {
        public override void SetInnerDevice(Assad.MHconfigTypeDevice innerDevice)
        {
        }

        public override Assad.DeviceType GetStates()
        {
            Assad.DeviceType deviceType = new Assad.DeviceType();
            deviceType.deviceId = DeviceId;
            List<Assad.DeviceTypeState> states = new List<Assad.DeviceTypeState>();

            Assad.DeviceTypeState configurationState = new Assad.DeviceTypeState();
            configurationState.state = "Конфигурация";
            configurationState.value = "Норма";
            states.Add(configurationState);

            List<StateTypeCounter> counters = new List<StateTypeCounter>();
            counters.Add(new StateTypeCounter() { StateType = StateType.Alarm });
            counters.Add(new StateTypeCounter() { StateType = StateType.Failure });
            counters.Add(new StateTypeCounter() { StateType = StateType.Info });
            //counters.Add(new StateTypeCounter() { StateType = StateType.No });
            counters.Add(new StateTypeCounter() { StateType = StateType.Norm });
            counters.Add(new StateTypeCounter() { StateType = StateType.Off });
            counters.Add(new StateTypeCounter() { StateType = StateType.Service });
            counters.Add(new StateTypeCounter() { StateType = StateType.Unknown });
            counters.Add(new StateTypeCounter() { StateType = StateType.Warning });

            foreach (DeviceState deviceState in FiresecManager.CurrentStates.DeviceStates)
            {
                StateType stateType = deviceState.State.StateType;
                StateTypeCounter stateTypeCounter = counters.FirstOrDefault(x => x.StateType == stateType);
                if (stateTypeCounter != null)
                {
                    stateTypeCounter.Count++;
                }
            }

            StateTypeCounter counter;

            Assad.DeviceTypeState state1 = new Assad.DeviceTypeState();
            counter = counters.FirstOrDefault(x => x.StateType == StateType.Alarm);
            state1.state = "Тревога";
            state1.value = (counter.Count > 0) ? "Есть" : "Нет"; 
            states.Add(state1);

            Assad.DeviceTypeState state2 = new Assad.DeviceTypeState();
            counter = counters.FirstOrDefault(x => x.StateType == StateType.Warning);
            state2.state = "Внимание (предтревожное)";
            state2.value = (counter.Count > 0) ? "Есть" : "Нет"; 
            states.Add(state2);

            Assad.DeviceTypeState state3 = new Assad.DeviceTypeState();
            counter = counters.FirstOrDefault(x => x.StateType == StateType.Failure);
            state3.state = "Неисправность";
            state3.value = (counter.Count > 0) ? "Есть" : "Нет"; 
            states.Add(state3);

            Assad.DeviceTypeState state4 = new Assad.DeviceTypeState();
            counter = counters.FirstOrDefault(x => x.StateType == StateType.Service);
            state4.state = "Требуется обслуживание";
            state4.value = (counter.Count > 0) ? "Есть" : "Нет";
            states.Add(state4);

            Assad.DeviceTypeState state5 = new Assad.DeviceTypeState();
            counter = counters.FirstOrDefault(x => x.StateType == StateType.Off);
            state5.state = "Обход устройств";
            state5.value = (counter.Count > 0) ? "Есть" : "Нет";
            states.Add(state5);

            Assad.DeviceTypeState state6 = new Assad.DeviceTypeState();
            counter = counters.FirstOrDefault(x => x.StateType == StateType.Unknown);
            state6.state = "Неопределено";
            state6.value = (counter.Count > 0) ? "Есть" : "Нет";
            states.Add(state6);

            Assad.DeviceTypeState state7 = new Assad.DeviceTypeState();
            counter = counters.FirstOrDefault(x => x.StateType == StateType.Info);
            state7.state = "Норма(*)";
            state7.value = (counter.Count > 0) ? "Есть" : "Нет";
            states.Add(state7);

            Assad.DeviceTypeState state8 = new Assad.DeviceTypeState();
//            counter = counters.FirstOrDefault(x => x.StateType == StateType.Failure);
            state8.state = "Норма";
            state8.value = "Нет";
            states.Add(state8);

            deviceType.state = states.ToArray();
            return deviceType;
        }

        public override void FireEvent(string eventName)
        {
            Assad.CPeventType eventType = new Assad.CPeventType();

            eventType.deviceId = DeviceId;
            eventType.eventTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            eventType.eventId =  eventName;
            eventType.alertLevel = "0";

            List<StateTypeCounter> counters = new List<StateTypeCounter>();
       //Alarm = 0,
       // Warning = 1,
       // Failure = 2,
       // Service = 3,
       // Off = 4,
       // Unknown = 5,
       // Info = 6,
       // Norm = 7,
       // No = 8
            State state_alarm = new State(0);
            State state_warning = new State(1);
            State state_failure = new State(2);
            State state_service = new State(3);
            State state_off = new State(4);
            State state_unknown = new State(5);
            State state_info = new State(6);
            State state_norm = new State(7);
            State state_no = new State(8);
            counters.Add(new StateTypeCounter() { StateType = StateType.Alarm, State = state_alarm });
            counters.Add(new StateTypeCounter() { StateType = StateType.Failure, State = state_failure });
            counters.Add(new StateTypeCounter() { StateType = StateType.Info, State = state_info });
            //counters.Add(new StateTypeCounter() { StateType = StateType.No });
            counters.Add(new StateTypeCounter() { StateType = StateType.Norm, State = state_norm });
            counters.Add(new StateTypeCounter() { StateType = StateType.Off, State = state_off });
            counters.Add(new StateTypeCounter() { StateType = StateType.Service, State = state_service });
            counters.Add(new StateTypeCounter() { StateType = StateType.Unknown, State = state_unknown });
            counters.Add(new StateTypeCounter() { StateType = StateType.Warning, State = state_warning });

            foreach (DeviceState deviceState in FiresecManager.CurrentStates.DeviceStates)
            {
                StateType stateType = deviceState.State.StateType;
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
                AlarmState.state = stateTypeCounter.State.ToString();
                AlarmState.value = (stateTypeCounter.Count > 0) ? "Есть" : "Нет";
                states.Add(AlarmState);
            }

            eventType.state = states.ToArray();

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

    class StateTypeCounter
    {
        public StateType StateType { get; set; }
        public State State{ get; set;}
        public int Count { get; set; }
    }
}
