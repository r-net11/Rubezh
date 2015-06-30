using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class GlobalVariableViewModel : BaseViewModel
	{
		public GlobalVariable GlobalVariable { get; set; }

		public GlobalVariableViewModel(GlobalVariable globalVariable)
		{
			GlobalVariable = globalVariable;
		}

		public string Name
		{
			get { return GlobalVariable.Name; }
			set
			{
				GlobalVariable.Name = value;
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public int Value
		{
			get { return GlobalVariable.Value; }
			set
			{
				GlobalVariable.Value = value;
				OnPropertyChanged(() => Value);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public void Update(GlobalVariable globalVariable)
		{
			GlobalVariable = globalVariable;
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Value);
		}
	}
}