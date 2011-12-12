using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class PlanViewModel : TreeBaseViewModel<PlanViewModel>
    {
        public Plan Plan;
        public List<DeviceState> DeviceStates;
        StateType _selfState;

        public PlanViewModel(Plan plan, ObservableCollection<PlanViewModel> source)
        {
            Plan = plan;
            Source = source;
            DeviceStates = new List<DeviceState>();
            foreach (var elementDevice in plan.ElementDevices)
            {
                var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == elementDevice.DeviceUID);
                if (deviceState != null)
                {
                    DeviceStates.Add(deviceState);
                    deviceState.StateChanged += new Action(UpdateSelfState);
                }
            }

            UpdateSelfState();
        }

        public string Caption
        {
            get { return Plan.Caption; }
        }

        public string Description
        {
            get { return Plan.Description; }
        }

        StateType _stateType;
        public StateType StateType
        {
            get { return _stateType; }
            set
            {
                _stateType = value;
                ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Publish(Plan.UID);
                OnPropertyChanged("StateType");
            }
        }

        public void UpdateSelfState()
        {
            int minPriority = 8;

            foreach (var deviceState in DeviceStates)
            {
                int priority = (int) deviceState.StateType;
                if (priority < minPriority)
                    minPriority = priority;
            }
            _selfState = (StateType) minPriority;

            UpdateState();
        }

        public void UpdateState()
        {
            int minPriority = (int) _selfState;

            foreach (var planViewModel in Children)
            {
                int priority = (int) planViewModel.StateType;
                if (priority < minPriority)
                    minPriority = priority;
            }
            StateType = (StateType) minPriority;

            if (Parent != null)
            {
                Parent.UpdateState();
            }
        }
    }
}