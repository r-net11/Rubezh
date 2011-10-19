using FiresecAPI.Models;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class PlanViewModel : BaseViewModel
    {
        public Plan Plan { get; private set; }

        public PlanViewModel(Plan plan)
        {
            Plan = plan;
        }

        public void Update()
        {
            OnPropertyChanged("Plan");
        }
    }
}