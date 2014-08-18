using System.Linq;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class VariableDetailsViewModel : SaveCancelDialogViewModel
	{
		public Variable Variable { get; private set; }
		public bool IsEditMode { get; private set; }
		public VariableDetailsViewModel(bool isArgument = false, bool isGlobal = false)
		{
			var defaultName = "Локальная переменная";
			var title = "Добавить переменную";
			if (isArgument)
			{
				defaultName = "Аргумент";
				title = "Добавить аргумент";
			}
			if (isGlobal)
			{
				defaultName = "Глобальная переменная";
				title = "Добавить глобальную переменную";
			}
			Title = title;
			Variable = new Variable(defaultName);
			Variable.IsGlobal = isGlobal;
			Variables = new ObservableCollection<VariableViewModel>
			{
				new VariableViewModel(defaultName, ValueType.Integer),
				new VariableViewModel(defaultName, ValueType.Boolean),
				new VariableViewModel(defaultName, ValueType.String),
				new VariableViewModel(defaultName, ValueType.DateTime),
				new VariableViewModel(defaultName, ValueType.Object)
			};
			SelectedVariable = Variables.FirstOrDefault();
			Name = defaultName;
			IsEditMode = false;
		}

		public VariableDetailsViewModel(Variable variable, bool isArgument = false)
		{
			var title = "Редактировать переменную";
			if (isArgument)
			{
				title = "Редактировать аргумент";
			}
			Title = title;
			Variable = new Variable(variable);
			Variables = new ObservableCollection<VariableViewModel>
			{
				 new VariableViewModel(variable)
			};
			SelectedVariable = Variables.FirstOrDefault(x => x.ValueType == variable.ValueType);
			if (SelectedVariable != null)
			{
				Name = SelectedVariable.Name;
				IsList = SelectedVariable.IsList;
			}
			IsEditMode = true;
		}
	
		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				OnPropertyChanged(()=>SelectedVariable);
			}
		}

		ObservableCollection<VariableViewModel> _variables;
		public ObservableCollection<VariableViewModel> Variables
		{
			get { return _variables; }
			set
			{
				_variables = value;
				OnPropertyChanged(()=>Variables);
			}
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

		bool _isList;
		public bool IsList
		{
			get { return _isList; }
			set
			{
				_isList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}
			Variable.Name = SelectedVariable.Name;
			Variable.DefaultBoolValue = SelectedVariable.DefaultBoolValue;
			Variable.DefaultDateTimeValue = SelectedVariable.DefaultDateTimeValue;
			Variable.DefaultIntValue = SelectedVariable.DefaultIntValue;
			Variable.Name = Name;
			Variable.ObjectType = SelectedVariable.ObjectType;
			Variable.DefaultStringValue = SelectedVariable.DefaultStringValue;
			Variable.ValueType = SelectedVariable.ValueType;
			Variable.IsList = IsList;
			return base.Save();
		}
	}	
}