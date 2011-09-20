using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class PlanViewModel : BaseViewModel
    {
        public PlanViewModel(Plan plan)
        {
            Plan = plan;
            BuildTree(plan.Children);

            Children = new ObservableCollection<PlanViewModel>();
            ElementSubPlans = new ObservableCollection<SubPlanViewModel>();
        }

        public Plan Plan { get; private set; }
        public ObservableCollection<PlanViewModel> Children { get; private set; }
        public ObservableCollection<SubPlanViewModel> ElementSubPlans { get; private set; }

        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Plan.Name))
                    return "План без названия";
                return Plan.Name;
            }
        }
        public double Width { get { return Plan.Width; } }
        public double Height { get { return Plan.Height; } }

        public void BuildTree(List<Plan> plans)
        {
            if (plans.IsNotNullOrEmpty())
            {
                foreach (var plan in plans)
                {
                    plan.Parent = Plan;
                    Children.Add(new PlanViewModel(plan));
                }
            }
        }

        public void Update()
        {
            OnPropertyChanged("Plan");
        }
    }
}