using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using PlansModule.Models;
using System.Collections.ObjectModel;

namespace PlansModule.ViewModels
{
    public class PlanTreeItemViewModel : TreeBaseViewModel<PlanTreeItemViewModel>
    {
        public PlanTreeItemViewModel()
        {
            Children = new ObservableCollection<PlanTreeItemViewModel>();
        }

        public Plan plan;

        public void Initialize(Plan plan, ObservableCollection<PlanTreeItemViewModel> source)
        {
            this.plan = plan;
            Source = source;
        }

        public string Name
        {
            get { return plan.Caption; }
        }

        public string State
        {
            get { return "Норма"; }
        }
    }
}
