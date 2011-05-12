using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using PlansModule.Models;
using System.Collections.ObjectModel;
using Firesec;
using FiresecClient;
using PlansModule.Events;

namespace PlansModule.ViewModels
{
    public class PlanViewModel : TreeBaseViewModel<PlanViewModel>
    {
        public Plan _plan;
        public List<DeviceState> _deviceStates;
        string _selfState;

        public void Initialize(Plan plan, ObservableCollection<PlanViewModel> source)
        {
            _plan = plan;
            Source = source;
            _deviceStates = new List<DeviceState>();
            foreach (var elementDevice in plan.ElementDevices)
            {
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == elementDevice.Path);
                _deviceStates.Add(deviceState);
                deviceState.StateChanged += new Action(UpdateSelfState);
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

        string _state;
        public string State
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

            foreach(var deviceState in _deviceStates)
            {
                int priority = StateHelper.NameToPriority(deviceState.State);
                if (priority < minPriority)
                    minPriority = priority;
            }
            _selfState = StateHelper.GetState(minPriority);

            UpdateState();
        }

        public void UpdateState()
        {
            int minPriority = StateHelper.NameToPriority(_selfState);

            foreach (var planViewModel in Children)
            {
                int priority = StateHelper.NameToPriority(planViewModel.State);
                if (priority < minPriority)
                    minPriority = priority;
            }
            State = StateHelper.GetState(minPriority);

            if (Parent != null)
            {
                Parent.UpdateState();
            }
        }
    }
}
