using FiresecAPI.Models;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class PlanViewModel : TreeItemViewModel<PlanViewModel>
    {
        public Plan Plan { get; private set; }

        public PlanViewModel(Plan plan)
        {
            Plan = plan;
        }

        public void Update()
        {
            IsExpanded = false;
            IsExpanded = true;
            OnPropertyChanged("HasChildren");
            OnPropertyChanged("Plan");
        }
    }
}