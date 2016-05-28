using StrazhAPI.Automation;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;

namespace AutomationModule.ViewModels
{
	public class VariableViewModel : BaseViewModel
	{
		public Variable Variable { get; set; }
		public ExplicitValueViewModel ExplicitValue { get; protected set; }
		public ObservableCollection<ExplicitValueViewModel> ExplicitValues { get; set; }
		public bool IsEditMode { get; set; }

		public VariableViewModel(Variable variable)
		{
			Variable = variable;
			ExplicitValue = new ExplicitValueViewModel(variable.ExplicitValue);
			ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();
			foreach (var explicitValue in variable.ExplicitValues)
				ExplicitValues.Add(new ExplicitValueViewModel(explicitValue));
			OnPropertyChanged(() => ExplicitValues);
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>();
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<ExplicitValueViewModel>(OnRemove);
			ChangeCommand = new RelayCommand<ExplicitValueViewModel>(OnChange);
		}

		public VariableViewModel(Argument argument)
		{
			Variable = new Variable
			{
				ExplicitType = argument.ExplicitType,
				EnumType = argument.EnumType,
				ObjectType = argument.ObjectType
			};
			ExplicitValue = new ExplicitValueViewModel(argument.ExplicitValue);
			ExplicitValues = new ObservableCollection<ExplicitValueViewModel>();

			foreach (var explicitValue in argument.ExplicitValues)
				ExplicitValues.Add(new ExplicitValueViewModel(explicitValue));

			OnPropertyChanged(() => ExplicitValues);
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>();
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<ExplicitValueViewModel>(OnRemove);
			ChangeCommand = new RelayCommand<ExplicitValueViewModel>(OnChange);
		}

		public string Name
		{
			get { return Variable.Name; }
			set
			{
				Variable.Name = value;
				OnPropertyChanged(() => Name);
			}
		}

		public ExplicitType ExplicitType
		{
			get { return Variable.ExplicitType; }
			set
			{
				Variable.ExplicitType = value;
				OnPropertyChanged(() => ExplicitValues);
				OnPropertyChanged(() => ExplicitType);
			}
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }
		public EnumType SelectedEnumType
		{
			get { return Variable.EnumType; }
			set
			{
				Variable.EnumType = value;
				OnPropertyChanged(() => SelectedEnumType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		public ObjectType SelectedObjectType
		{
			get { return Variable.ObjectType; }
			set
			{
				Variable.ObjectType = value;
				OnPropertyChanged(() => SelectedObjectType);
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => ExplicitValue);
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var explicitValueViewModel = new ExplicitValueViewModel(new ExplicitValue());
			if (ExplicitType == ExplicitType.Object)
				ProcedureHelper.SelectObject(SelectedObjectType, explicitValueViewModel);
			ExplicitValues.Add(explicitValueViewModel);
			Variable.ExplicitValues.Add(explicitValueViewModel.ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
		}

		public RelayCommand<ExplicitValueViewModel> RemoveCommand { get; private set; }
		void OnRemove(ExplicitValueViewModel explicitValueViewModel)
		{
			ExplicitValues.Remove(explicitValueViewModel);
			Variable.ExplicitValues.Remove(explicitValueViewModel.ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
		}

		public RelayCommand<ExplicitValueViewModel> ChangeCommand { get; private set; }
		void OnChange(ExplicitValueViewModel explicitValueViewModel)
		{
			ProcedureHelper.SelectObject(SelectedObjectType, ExplicitValue);
			OnPropertyChanged(() => ExplicitValues);
			OnPropertyChanged(() => ExplicitValue);
		}
	}
}