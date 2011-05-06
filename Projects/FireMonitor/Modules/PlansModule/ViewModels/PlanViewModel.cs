using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using PlansModule.Models;
using System.Collections.ObjectModel;
using Firesec;
using FiresecClient;

namespace PlansModule.ViewModels
{
    public class PlanViewModel : TreeBaseViewModel<PlanViewModel>
    {
        public PlanViewModel()
        {
            Children = new ObservableCollection<PlanViewModel>();
            SelfState = "Норма";
            State = "Норма";
        }

        public Plan plan;
        public List<DeviceState> deviceStates;

        public void Initialize(Plan plan, ObservableCollection<PlanViewModel> source)
        {
            this.plan = plan;
            Source = source;
            deviceStates = new List<DeviceState>();
            foreach (var elementDevice in plan.ElementDevices)
            {
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == elementDevice.Path);
                deviceStates.Add(deviceState);
                deviceState.StateChanged += new Action(UpdateSelfState);
            }
        }

        public string Name
        {
            get { return plan.Caption; }
        }

        public string SelfState { get; set; }

        string state;
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }

        public void UpdateSelfState()
        {
            int minPriority = 7;

            foreach(var deviceState in deviceStates)
            {
                int priority = StateHelper.NameToPriority(deviceState.State);
                if (priority < minPriority)
                    minPriority = priority;
            }
            SelfState = StateHelper.GetState(minPriority);

            UpdateState();
        }

        public void UpdateState()
        {
            int minPriority = StateHelper.NameToPriority(SelfState);

            foreach (var planViewModel in Children)
            {
                int priority = StateHelper.NameToPriority(planViewModel.State);
                if (priority < minPriority)
                    minPriority = priority;
            }
            State = StateHelper.GetState(minPriority);

            //UpdateSubPlans();

            if (Parent != null)
            {
                Parent.UpdateState();
            }
        }

        //public void UpdateSubPlans()
        //{
        //    //if (SubPlans != null)
        //    //    foreach (ElementSubPlanViewModel subPlan in SubPlans)
        //    //    {
        //    //        PlanViewModel planViewModel = Children.FirstOrDefault(x => x.Name == subPlan.Name);
        //    //        if (planViewModel != null)
        //    //        {
        //    //            subPlan.Update(planViewModel.State);
        //    //        }
        //    //    }
        //}
    }
}
