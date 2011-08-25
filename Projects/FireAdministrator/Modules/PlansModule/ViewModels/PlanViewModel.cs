using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
using System.Collections.ObjectModel;

namespace PlansModule.ViewModels
{
    public class PlanViewModel : BaseViewModel
    {

        public PlanViewModel(Plan plan)
        {
            Plan = plan;
            Name = plan.Name;
            Width = plan.Width;
            Height = plan.Height;
        }

        public void AddChild(PlanViewModel parent, PlanViewModel child)
        {
            if (parent.Children == null) parent.Children = new ObservableCollection<PlanViewModel>();
            parent.Children.Add(child);
        }

        public void AddSubPlan(PlanViewModel parent, SubPlanViewModel subplan)
        {
            if (parent.SubPlans == null) parent.SubPlans = new ObservableCollection<SubPlanViewModel>();
            parent.SubPlans.Add(subplan);
        }

        public Plan Plan { get; private set; }
        
        private ObservableCollection<PlanViewModel> _children;
        public ObservableCollection<PlanViewModel> Children
        {
            get
            {
                if (_children == null)
                    _children = new ObservableCollection<PlanViewModel>();
                return _children;
            }
            set { _children = value; }
        }

        private ObservableCollection<SubPlanViewModel> _subPlans;
        private ObservableCollection<SubPlanViewModel> SubPlans
        {
            get
            {
                if (_subPlans == null)
                    _subPlans = new ObservableCollection<SubPlanViewModel>();
                return _subPlans;
            }
            set { _subPlans = value; }
        }


        public string Name { get; private set; }
        public double Width { get; private set; }
        public double Height { get; private set; }

        public void Update()
        {
            OnPropertyChanged("Plan");
        }
    }
}
