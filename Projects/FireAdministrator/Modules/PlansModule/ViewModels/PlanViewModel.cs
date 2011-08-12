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
        }
        public Plan Plan { get; private set; }
        public string Name { get; private set; }
        public ObservableCollection<PlanViewModel> Children { get; private set; }

        public void AddChildren(PlanViewModel planViewModel)
        {
            if (Children == null) Children = new ObservableCollection<PlanViewModel>();
            Children.Add(planViewModel);
        }
        public void Update()
        {
            OnPropertyChanged("Plan");
        }
    }
}
