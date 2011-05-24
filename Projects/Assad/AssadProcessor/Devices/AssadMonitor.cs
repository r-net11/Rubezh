using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecClient;
using System.Diagnostics;
namespace AssadProcessor.Devices
{
    public class AssadMonitor : AssadBase
    {

        private List<StateTypeCounter> prevcounters;
        public AssadMonitor()
        {
            if (prevcounters == null) prevcounters = new List<StateTypeCounter>();
        
        }
        
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

//            List<StateTypeCounter> counters = new List<StateTypeCounter>();
            prevcounters.Add(new StateTypeCounter() { StateType = StateType.Alarm });
            prevcounters.Add(new StateTypeCounter() { StateType = StateType.Failure });
            prevcounters.Add(new StateTypeCounter() { StateType = StateType.Info });
            //counters.Add(new StateTypeCounter() { StateType = StateType.No });
            prevcounters.Add(new StateTypeCounter() { StateType = StateType.Norm });
            prevcounters.Add(new StateTypeCounter() { StateType = StateType.Off });
            prevcounters.Add(new StateTypeCounter() { StateType = StateType.Service });
            prevcounters.Add(new StateTypeCounter() { StateType = StateType.Unknown });
            prevcounters.Add(new StateTypeCounter() { StateType = StateType.Warning });

            foreach (DeviceState deviceState in FiresecManager.CurrentStates.DeviceStates)
            {
                StateType stateType = deviceState.State.StateType;
                StateTypeCounter stateTypeCounter = prevcounters.FirstOrDefault(x => x.StateType == stateType);
                if (stateTypeCounter != null)
                {
                    if (stateTypeCounter.StateType == StateType.Norm) continue;
                    stateTypeCounter.Count++;
                }
            }

           //// отладочная информация
           // foreach(StateTypeCounter dcount in prevcounters)
           // {
           //     string str = "DeviceId="+ DeviceId + " -- " + dcount.StateType.ToString() + "  " + dcount.Count.ToString();
           //     Trace.WriteLine(str);
           // }


            StateTypeCounter counter;

            Assad.DeviceTypeState state1 = new Assad.DeviceTypeState();
            counter = prevcounters.FirstOrDefault(x => x.StateType == StateType.Alarm);
            state1.state = "Тревога";
            state1.value = (counter.Count > 0) ? "Есть" : "Нет"; 
            states.Add(state1);

            Assad.DeviceTypeState state2 = new Assad.DeviceTypeState();
            counter = prevcounters.FirstOrDefault(x => x.StateType == StateType.Warning);
            state2.state = "Внимание (предтревожное)";
            state2.value = (counter.Count > 0) ? "Есть" : "Нет"; 
            states.Add(state2);

            Assad.DeviceTypeState state3 = new Assad.DeviceTypeState();
            counter = prevcounters.FirstOrDefault(x => x.StateType == StateType.Failure);
            state3.state = "Неисправность";
            state3.value = (counter.Count > 0) ? "Есть" : "Нет"; 
            states.Add(state3);

            Assad.DeviceTypeState state4 = new Assad.DeviceTypeState();
            counter = prevcounters.FirstOrDefault(x => x.StateType == StateType.Service);
            state4.state = "Требуется обслуживание";
            state4.value = (counter.Count > 0) ? "Есть" : "Нет";
            states.Add(state4);

            Assad.DeviceTypeState state5 = new Assad.DeviceTypeState();
            counter = prevcounters.FirstOrDefault(x => x.StateType == StateType.Off);
            state5.state = "Обход устройств";
            state5.value = (counter.Count > 0) ? "Есть" : "Нет";
            states.Add(state5);

            Assad.DeviceTypeState state6 = new Assad.DeviceTypeState();
            counter = prevcounters.FirstOrDefault(x => x.StateType == StateType.Unknown);
            state6.state = "Неопределено";
            state6.value = (counter.Count > 0) ? "Есть" : "Нет";
            states.Add(state6);

            Assad.DeviceTypeState state7 = new Assad.DeviceTypeState();
            counter = prevcounters.FirstOrDefault(x => x.StateType == StateType.Info);
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
            State state_alarm   =   new State(0);
            State state_warning =   new State(1);
            State state_failure =   new State(2);
            State state_service =   new State(3);
            State state_off     =   new State(4);
            State state_unknown =   new State(5);
            State state_info    =   new State(6);
            State state_norm    =   new State(7);
            State state_no      =   new State(8);
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

            int diffcount = 0;

            foreach (StateTypeCounter counter in prevcounters)
            {
                StateTypeCounter tempcounter = counters.FirstOrDefault(x => x.StateType == counter.StateType);
                    if(tempcounter != null)
                    {
                        if (tempcounter.StateType == StateType.Norm) continue; 
                        if(tempcounter.Count != counter.Count)
                        {
                            //// отладочная информация
                            //string str = "DeviceId="+ DeviceId + " -- " + tempcounter.StateType.ToString() + "cтарое значение:" + counter.Count.ToString() + "   Новое значение:" + tempcounter.Count.ToString();
                            //Trace.WriteLine(str);
                            diffcount++;
                            counter.Count = tempcounter.Count;
                        }
                    }
            }
    
            if(diffcount == 0) return;


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
