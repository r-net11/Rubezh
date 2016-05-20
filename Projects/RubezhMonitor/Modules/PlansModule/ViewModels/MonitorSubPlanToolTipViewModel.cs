using Infrastructure.Common.Windows.ViewModels;

namespace PlansModule.ViewModels
{
	public class MonitorSubPlanToolTipViewModel : BaseViewModel
	{
		public MonitorSubPlanToolTipViewModel(PlanViewModel planViewModel)
		{
			PlanViewModel = planViewModel;
		}

		PlanViewModel planViewModel;
		public PlanViewModel PlanViewModel
		{
			get { return planViewModel; }
			set
			{
				planViewModel = value;
				OnPropertyChanged(() => PlanViewModel);
			}
		}
	}
}