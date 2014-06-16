using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class VariableViewModel : BaseViewModel
	{
		public Variable Variable { get; private set; }

		public VariableViewModel(Variable variable)
		{
			Variable = variable;
		}
		public VariableViewModel()
		{
			Variable = new Variable();
		}

		public string Name
		{
			get { return Variable.Name; }
			set
			{
				Variable.Name = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => Name);
			}
		}

		public int Value
		{
			get { return Variable.Value; }
			set
			{
				Variable.Value = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged("Value");
			}
		}

		public VariableType VariableType
		{
			get { return Variable.VariableType; }
			set
			{
				Variable.VariableType = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged("VariableType");
			}
		}
	}
}
