using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace PlansModule.ViewModels
{
	public class MonitorSubPlanViewModel: BaseViewModel
	{
		public MonitorSubPlanViewModel(PlanViewModel planViewModel)
		{
			PlanViewModel = planViewModel;
		}

		PlanViewModel planViewModel;
		public PlanViewModel PlanViewModel
		{
			get{ return planViewModel; }
			set
			{
				planViewModel = value;
				OnPropertyChanged(() => PlanViewModel);
			}
		}
	}
}
