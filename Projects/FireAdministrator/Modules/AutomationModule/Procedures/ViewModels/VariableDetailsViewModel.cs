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

		public VariableDetailsViewModel(bool isArgument = false)
		{
			var defaultName = "Локальная переменная";
			var title = "Добавить переменную";
			if (isArgument)
			{
				defaultName = "Аргумент";
				title = "Добавить аргумент";
			}
			Title = title;
			Variable = new Variable(defaultName);
			Variables = new ObservableCollection<VariableViewModel>
			{
				new VariableViewModel(defaultName, VariableType.Integer),
				new VariableViewModel(defaultName, VariableType.Boolean),
				new VariableViewModel(defaultName, VariableType.String),
				new VariableViewModel(defaultName, VariableType.DateTime),
				new VariableViewModel(defaultName, VariableType.Object)
			};
			SelectedVariable = Variables.FirstOrDefault();
			Name = defaultName;
		}

		public VariableDetailsViewModel(Variable variable, bool isArgument = false)
		{
			var defaultName = "Локальная переменная";
			var title = "Редактировать переменную";
			if (isArgument)
			{
				defaultName = "Аргумент";
				title = "Редактировать аргумент";
			}
			Title = title;
			Variable = new Variable(variable);
			Variables = new ObservableCollection<VariableViewModel>
			{
				variable.VariableType != VariableType.Integer
					? new VariableViewModel(defaultName, VariableType.Integer) : new VariableViewModel(variable),
				variable.VariableType != VariableType.Boolean
					? new VariableViewModel(defaultName, VariableType.Boolean) : new VariableViewModel(variable),
				variable.VariableType != VariableType.String
					? new VariableViewModel(defaultName, VariableType.String) : new VariableViewModel(variable),
				variable.VariableType != VariableType.DateTime
					? new VariableViewModel(defaultName, VariableType.DateTime) : new VariableViewModel(variable),
				variable.VariableType != VariableType.Object
					? new VariableViewModel(defaultName, VariableType.Object) : new VariableViewModel(variable)
			};
			SelectedVariable = Variables.FirstOrDefault(x => x.VariableType == variable.VariableType);
			if (SelectedVariable != null)
			{
				Name = SelectedVariable.Name;
				IsList = SelectedVariable.IsList;
			}
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
			Variable.BoolValue = SelectedVariable.BoolValue;
			Variable.DateTimeValue = SelectedVariable.DateTimeValue;
			Variable.IntValue = SelectedVariable.IntValue;
			Variable.Name = Name;
			Variable.ObjectType = SelectedVariable.ObjectType;
			Variable.StringValue = SelectedVariable.StringValue;
			Variable.VariableType = SelectedVariable.VariableType;
			Variable.IsList = IsList;
			return base.Save();
		}
	}	
}