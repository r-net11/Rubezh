using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlansModule.ViewModels
{
	public class PlansTreeViewModel
	{
		public PlansTreeViewModel(PlansViewModel plansViewModel)
		{
			PlansViewModel = plansViewModel;
		}

		public PlansViewModel PlansViewModel { get; private set; }
	}
}
