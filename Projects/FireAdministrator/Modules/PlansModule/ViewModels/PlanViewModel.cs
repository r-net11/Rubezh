using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common;

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
            if (parent.ElementSubPlans == null) parent.ElementSubPlans = new ObservableCollection<SubPlanViewModel>();
            parent.ElementSubPlans.Add(subplan);
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
        private ObservableCollection<SubPlanViewModel> _elementZones;
        private ObservableCollection<ZoneViewModel> ElementZones;
        private ObservableCollection<SubPlanViewModel> _elementSubPlans;
        
        private ObservableCollection<SubPlanViewModel> ElementSubPlans
        {
            get
            {
                if (_elementSubPlans == null)
                    _elementSubPlans = new ObservableCollection<SubPlanViewModel>();
                return _elementSubPlans;
            }
            set { _elementSubPlans = value; }
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
