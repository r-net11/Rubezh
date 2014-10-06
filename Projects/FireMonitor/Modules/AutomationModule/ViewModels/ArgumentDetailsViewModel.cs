using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ArgumentDetailsViewModel : SaveCancelDialogViewModel
	{
		public Argument Argument { get; private set; }
		public ExplicitValuesViewModel ExplicitValuesViewModel { get; protected set; }

		public ArgumentDetailsViewModel(Argument argument, bool isList)
		{
			Title = "Редактировать аргумент";
			ExplicitValuesViewModel = new ExplicitValuesViewModel(argument.ExplicitValue, argument.ExplicitValues, isList, argument.ExplicitType, argument.EnumType, argument.ObjectType);
		}

		protected override bool Save()
		{
			Argument = new Argument();
			Argument.VariableScope = VariableScope.ExplicitValue;
			Argument.ExplicitType = ExplicitValuesViewModel.ExplicitType;
			Argument.EnumType = ExplicitValuesViewModel.EnumType;
			Argument.ObjectType = ExplicitValuesViewModel.ObjectType;
			Argument.ExplicitValue = ExplicitValuesViewModel.ExplicitValue.ExplicitValue;
			foreach (var explicitValue in ExplicitValuesViewModel.ExplicitValues)
				Argument.ExplicitValues.Add(explicitValue.ExplicitValue);
			return base.Save();
		}
	}
}
