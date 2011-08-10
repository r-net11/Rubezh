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
        State _selfState;

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

        State _state;
        public State State
        {
            get { return _state; }
            set
            {
                _state = value;
                ServiceFactory.Events.GetEvent<PlanStateChangedEvent>().Publish(_plan.Name);
                OnPropertyChanged("State");
            }
        }

        public void UpdateSelfState()
        {
            int minPriority = 7;

            foreach (var deviceState in _deviceStates)
            {
                int priority = deviceState.State.Id;
                if (priority < minPriority)
                    minPriority = priority;
            }
            _selfState = new State() { Id = minPriority };

            UpdateState();
        }

        public void UpdateState()
        {
            int minPriority = _selfState.Id;

            foreach (var planViewModel in Children)
            {
                int priority = planViewModel.State.Id;
                if (priority < minPriority)
                    minPriority = priority;
            }
            State = new State() { Id = minPriority };

            if (Parent != null)
            {
                Parent.UpdateState();
            }
        }
    }
}