using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;

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
            var deviceType = new Assad.DeviceType();
            deviceType.deviceId = DeviceId;
            var states = new List<Assad.DeviceTypeState>();
            var configurationState = new Assad.DeviceTypeState();
            configurationState.state = "Конфигурация";
            configurationState.value = "Норма";
            states.Add(configurationState);

            foreach (var device in FiresecManager.Devices)
            {
                var stateType = device.DeviceState.StateType;
                var stateTypeCounter = _stateTypeCounters.FirstOrDefault(x => x.StateType == stateType);
                if (stateTypeCounter.StateType == StateType.Norm)
                    continue;

                stateTypeCounter.HasOne = true;
            }

            foreach (var counter in _stateTypeCounters)
            {
                var state = new Assad.DeviceTypeState();
                state.state = counter.StateType.ToDescription();
                state.value = counter.HasOne ? "Есть" : "Нет";
                states.Add(state);
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

            var counters = new List<StateTypeCounter>();

            for (int i = 0; i < 9; i++)
                counters.Add(new StateTypeCounter(i));

            foreach (var device in FiresecManager.Devices)
            {
                var stateType = device.DeviceState.StateType;
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

            var states = new List<Assad.CPeventTypeState>();

            foreach (var stateTypeCounter in counters)
            {
                var AlarmState = new Assad.CPeventTypeState();
                AlarmState.state = stateTypeCounter.StateType.ToDescription();
                AlarmState.value = (stateTypeCounter.HasOne == true) ? "Есть" : "Нет";
                states.Add(AlarmState);
            }

            eventType.state = states.ToArray();

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

    internal class StateTypeCounter
    {
        public StateType StateType { get; set; }
        public bool HasOne { get; set; }

        public StateTypeCounter(int i)
        {
            StateType = (StateType) i;
        }
    }
}