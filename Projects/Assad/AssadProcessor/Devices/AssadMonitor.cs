using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecClient;
using System.Diagnostics;
using FiresecClient.Models;

namespace AssadProcessor.Devices
{
    public class AssadMonitor : AssadBase
    {
        List<StateTypeCounter> _stateTypeCounters;

        public AssadMonitor()
        {
            _stateTypeCounters = new List<StateTypeCounter>();
            for (int i = 0; i < 9; i++)
            {
                _stateTypeCounters.Add(new StateTypeCounter(i));
            }
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

            foreach (var deviceState in FiresecManager.States.DeviceStates)
            {
                var stateType = deviceState.State.StateType;
                var stateTypeCounter = _stateTypeCounters.FirstOrDefault(x => x.StateType == stateType);
                if (stateTypeCounter.StateType == StateType.Norm)
                    continue;

                stateTypeCounter.HasOne = true;
            }

            foreach (var counter in _stateTypeCounters)
            {
                Assad.DeviceTypeState state = new Assad.DeviceTypeState();
                state.state = counter.State.StateType.ToString();
                state.value = counter.HasOne ? "Есть" : "Нет";
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
            eventType.eventId = eventName;
            eventType.alertLevel = "0";

            List<StateTypeCounter> counters = new List<StateTypeCounter>();

            for (int i = 0; i < 9; i++)
                counters.Add(new StateTypeCounter(i));

            foreach (var deviceState in FiresecManager.States.DeviceStates)
            {
                var stateType = deviceState.State.StateType;
                var stateTypeCounter = counters.FirstOrDefault(x => x.StateType == stateType);
                stateTypeCounter.HasOne = true;
            }

            bool diff = false;

            foreach (var counter in _stateTypeCounters)
            {
                var tempcounter = counters.FirstOrDefault(x => x.StateType == counter.StateType);

                if (tempcounter.StateType == StateType.Norm)
                    continue;

                if (tempcounter.HasOne != counter.HasOne)
                {
                    diff = true;
                    counter.HasOne = tempcounter.HasOne;
                }
            }

            if (diff == false)
                return;

            List<Assad.CPeventTypeState> states = new List<Assad.CPeventTypeState>();

            foreach (var stateTypeCounter in counters)
            {
                Assad.CPeventTypeState AlarmState = new Assad.CPeventTypeState();
                AlarmState.state = stateTypeCounter.State.ToString();
                AlarmState.value = (stateTypeCounter.HasOne == true) ? "Есть" : "Нет";
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
        public State State { get; set; }
        public bool HasOne { get; set; }

        public StateTypeCounter(int i)
        {
            State = new State(i);
            StateType = State.StateType;
        }
    }
}
