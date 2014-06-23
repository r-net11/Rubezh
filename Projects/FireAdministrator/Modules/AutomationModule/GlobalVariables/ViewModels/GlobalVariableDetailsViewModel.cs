using FiresecAPI.Automation;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

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
			Value = globalVariable.Value;
		}

		public GlobalVariableDetailsViewModel()
		{
			Title = "Добавить глобальную переменную";
			GlobalVariable = new GlobalVariable();
			Name = GlobalVariable.Name;
			Value = GlobalVariable.Value;
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

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}
			GlobalVariable.Name = Name;
			GlobalVariable.Value = Value;
			return base.Save();
		}
	}
}