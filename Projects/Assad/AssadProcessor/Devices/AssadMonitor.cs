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
        private List<StateTypeCounter> prevCounters;
        public AssadMonitor()
        {
            prevCounters = new List<StateTypeCounter>();
            for (int i = 0; i < 9; i++)
            {
                prevCounters.Add(new StateTypeCounter(i));
            }
            //// отладочная информация              
            //foreach (StateTypeCounter dcount in prevCounters)
            //{
            //    string str = "State: " + dcount.State.ToString() + "   StateType: " + dcount.StateType.ToString() + "  " + dcount.Count.ToString();
            //    Trace.WriteLine(str);
            //}
            //Trace.WriteLine(" - выход из конструктора - ");
        
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

            foreach (DeviceState deviceState in FiresecManager.CurrentStates.DeviceStates)
            {
                StateType stateType = deviceState.State.StateType;
                StateTypeCounter stateTypeCounter = prevCounters.FirstOrDefault(x => x.StateType == stateType);
                if (stateTypeCounter != null)
                {
                    if (stateTypeCounter.StateType == StateType.Norm) continue;
                    stateTypeCounter.Count = true;
                }
            }

           //// отладочная информация
            //Trace.WriteLine("--- prevCounter List ---");
            //foreach (StateTypeCounter dcount in prevCounters)
            //{
            //    string str = dcount.State.ToString() + "  " + dcount.Count.ToString();
            //    Trace.WriteLine(str);
            //}



            foreach (StateTypeCounter counter in prevCounters)
            {
                Assad.DeviceTypeState state = new Assad.DeviceTypeState();
                state.state = counter.State.StateType.ToString();
                state.value = (counter.Count == true) ? "Есть" : "Нет";
                states.Add(state);
            }


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

            for (int i = 0; i < 9; i++ )
                counters.Add(new StateTypeCounter(i));
            foreach (DeviceState deviceState in FiresecManager.CurrentStates.DeviceStates)
            {
                StateType stateType = deviceState.State.StateType;
                StateTypeCounter stateTypeCounter = counters.FirstOrDefault(x => x.StateType == stateType);
                if (stateTypeCounter != null)
                {
                    stateTypeCounter.Count = true;
                }
            }

            bool diff = false;

            foreach (StateTypeCounter counter in prevCounters)
            {
                StateTypeCounter tempcounter = counters.FirstOrDefault(x => x.StateType == counter.StateType);
                    if(tempcounter != null)
                    {
                        if (tempcounter.StateType == StateType.Norm) continue; 
                        if(tempcounter.Count != counter.Count)
                        {
                            ////// отладочная информация
                            //string str = "DeviceId="+ DeviceId + " -- " + tempcounter.StateType.ToString() + "cтарое значение:" + counter.Count.ToString() + "   Новое значение:" + tempcounter.Count.ToString();
                            //Trace.WriteLine(str);
                            diff = true;
                            counter.Count = tempcounter.Count;
                        }
                    }
            }
    
            if(diff == false) return;


            List<Assad.CPeventTypeState> states = new List<Assad.CPeventTypeState>();

            foreach (StateTypeCounter stateTypeCounter in counters)
            {
                Assad.CPeventTypeState AlarmState = new Assad.CPeventTypeState();
                AlarmState.state = stateTypeCounter.State.ToString();
                AlarmState.value = (stateTypeCounter.Count == true) ? "Есть" : "Нет";
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
        public bool Count { get; set; }

        public StateTypeCounter() { }
        
        public StateTypeCounter(int i)
        {
            State = new State(i);
            StateType = State.StateType;
        }

    
    }
}
