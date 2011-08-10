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
        public Plan _plan;
        public List<DeviceState> _deviceStates;
        StateType _selfState;

        public void Initialize(Plan plan, ObservableCollection<PlanViewModel> source)
        {
            _plan = plan;
            Source = source;
            _deviceStates = new List<DeviceState>();
            foreach (var elementDevice in plan.ElementDevices)
            {
                var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == elementDevice.Id);
                if (deviceState != null)
                {
                    _deviceStates.Add(deviceState);
                    deviceState.StateChanged += new Action(UpdateSelfState);
                }
            }

            UpdateSelfState();
        }

        public string Caption
        {
            get { return _plan.Caption; }
        }

        public string Description
        {
            get { return _plan.Description; }
        }

        StateType _stateType;
        public StateType StateType
        {
            get { return _stateType; }
            set
            {
                _stateType = value;
                ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Publish(_plan.Name);
                OnPropertyChanged("StateType");
            }
        }

        public void UpdateSelfState()
        {
            int minPriority = 8;

            foreach (var deviceState in _deviceStates)
            {
                int priority = (int)deviceState.StateType;
                if (priority < minPriority)
                    minPriority = priority;
            }
            _selfState = (StateType)minPriority;

            UpdateState();
        }

        public void UpdateState()
        {
            int minPriority = (int)_selfState;

            foreach (var planViewModel in Children)
            {
                int priority = (int)planViewModel.StateType;
                if (priority < minPriority)
                    minPriority = priority;
            }
            StateType = (StateType)minPriority;

            if (Parent != null)
            {
                Parent.UpdateState();
            }
        }
    }
}