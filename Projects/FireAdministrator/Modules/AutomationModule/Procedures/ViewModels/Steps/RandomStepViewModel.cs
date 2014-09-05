using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class RandomStepViewModel : BaseViewModel, IStepViewModel
	{
		Procedure Procedure { get; set; }
		public RandomArguments RandomArguments { get; private set; }

		public RandomStepViewModel(RandomArguments randomArguments, Procedure procedure)
		{
			RandomArguments = randomArguments;
			Procedure = procedure;
			UpdateContent();
		}

		int _value;
		public int Value
		{
			get { return _value; }
			set
			{
				_value = value;
				OnPropertyChanged(() => Value);
			}
		}

		public void UpdateContent()
		{

		}

		public string Description
		{
			get { return ""; }
		}
	}
}