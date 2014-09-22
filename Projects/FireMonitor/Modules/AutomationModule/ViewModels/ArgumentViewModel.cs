using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace AutomationModule.ViewModels
{
	public class ArgumentViewModel : VariableViewModel
	{
		public ArgumentViewModel(Variable argument)
			: base(argument)
		{
			ExplicitValue = new ExplicitValueViewModel(argument.ExplicitValue);
			ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
			foreach (var explicitValue in argument.ExplicitValues)
				ExplicitValues.Add(new ExplicitValueViewModel(explicitValue));
		}
	}
}