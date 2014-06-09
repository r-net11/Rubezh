using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class GlobalVariableDetailsViewModel : SaveCancelDialogViewModel
	{
		public GlobalVariable GlobalVariable { get; private set; }
		public GlobalVariableDetailsViewModel(GlobalVariable globalVariable)
		{
			Title = "Свойства глобальной переменной";
			GlobalVariable = globalVariable;
			Name = globalVariable.Name;
		}

		public GlobalVariableDetailsViewModel()
		{
			Title = "Добавить глобальную переменную";
			GlobalVariable = new GlobalVariable();
			Name = GlobalVariable.Name;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		protected override bool Save()
		{
			GlobalVariable.Name = Name;
			return base.Save();
		}
	}
}
