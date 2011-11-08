using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class PlanViewModel : TreeBaseViewModel<PlanViewModel>
    {
        public Plan Plan { get; private set; }

        public PlanViewModel(Plan plan, ObservableCollection<PlanViewModel> sourcePlans)
        {
            Children = new ObservableCollection<PlanViewModel>();
            Source = sourcePlans;
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