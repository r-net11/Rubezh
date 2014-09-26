using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;
using Infrastructure;

namespace AutomationModule.ViewModels
{
	public class ArgumentDetailsViewModel : SaveCancelDialogViewModel
	{
		bool automationChanged;
		public Argument Argument { get; private set; }
		public ExplicitValuesViewModel ExplicitValuesViewModel { get; protected set; }

		public ArgumentDetailsViewModel(Argument argument, bool isList)
		{
			automationChanged = ServiceFactory.SaveService.AutomationChanged;
			Title = "Редактировать аргумент";
			ExplicitValuesViewModel = new ExplicitValuesViewModel(argument.ExplicitValue, argument.ExplicitValues, isList, argument.ExplicitType, argument.EnumType, argument.ObjectType);
		}

		public override bool OnClosing(bool isCanceled)
		{
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			return base.OnClosing(isCanceled);
		}

		protected override bool Save()
		{
			Argument = new Argument();
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
