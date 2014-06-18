using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ConditionsViewModel : BaseViewModel
	{
		public Procedure Procedure { get; private set; }

		public ConditionsViewModel(Procedure procedure)
		{
			Procedure = procedure;
		}
	}
}