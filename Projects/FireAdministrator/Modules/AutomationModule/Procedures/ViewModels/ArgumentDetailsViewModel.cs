using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class ArgumentDetailsViewModel : VariableDetailsViewModel
	{
		public ArgumentDetailsViewModel(Variable variable) : base(variable)
		{

		}
	}
}
