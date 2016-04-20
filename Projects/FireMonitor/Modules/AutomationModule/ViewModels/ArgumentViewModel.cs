using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class ArgumentViewModel : BaseViewModel
	{
		public ExplicitValueViewModel ExplicitValue { get; protected set; }
		public Variable Argument { get; private set; }

		public ArgumentViewModel(Variable argument)
		{
			Argument = argument;
			ExplicitValue = new ExplicitValueViewModel((ExplicitValue)argument, true);
			ChangeCommand = new RelayCommand<ExplicitValueViewModel>(OnChange);
			Name = argument.Name;
			IsList = argument.IsList;
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

		public RelayCommand<ExplicitValueViewModel> ChangeCommand { get; private set; }
		void OnChange(ExplicitValueViewModel explicitValueViewModel)
		{
			if (IsList)
				ProcedureHelper.SelectObject(Argument.ObjectType, explicitValueViewModel);
			else
				ProcedureHelper.SelectObject(Argument.ObjectType, ExplicitValue);
			OnPropertyChanged(() => ExplicitValue);
		}

		public string ValueDescription
		{
			get
			{
				/*if (!IsList)
					return Argument.ExplicitValue == null ? "" : Argument.ExplicitValue.ToString();
				else
				{
					if (Argument.ExplicitValues.Count == 0)
						return "Пустой список";
					return String.Join(", ", Argument.ExplicitValues.Select(x => x.ToString()));
				}*/
				return "refactoring";
			}
		}
	}
}