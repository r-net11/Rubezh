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
		public RandomArguments RandomArguments { get; private set; }
		public ArithmeticParameterViewModel MaxValue { get; private set; }

		public RandomStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			RandomArguments = stepViewModel.Step.RandomArguments;
			MaxValue = new ArithmeticParameterViewModel(RandomArguments.MaxValue, stepViewModel.Update);
			MaxValue.ExplicitType = ExplicitType.Integer;
		}

		public override void UpdateContent()
		{
			MaxValue.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Integer && !x.IsList));
		}

		public override string Description
		{
			get { return ""; }
		}
	}
}