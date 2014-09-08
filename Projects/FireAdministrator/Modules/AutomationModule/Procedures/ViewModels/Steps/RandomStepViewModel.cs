using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class RandomStepViewModel : BaseStepViewModel
	{
		Procedure Procedure { get; set; }
		public RandomArguments RandomArguments { get; private set; }
		public ArgumentItemViewModel MaxValue { get; private set; }

		public RandomStepViewModel(RandomArguments randomArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			RandomArguments = randomArguments;
			MaxValue = new ArgumentItemViewModel(procedure, randomArguments.MaxValue, new List<FiresecAPI.Automation.ValueType>() { FiresecAPI.Automation.ValueType.Integer });
			Procedure = procedure;
			UpdateContent();
		}

		public void UpdateContent()
		{
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}