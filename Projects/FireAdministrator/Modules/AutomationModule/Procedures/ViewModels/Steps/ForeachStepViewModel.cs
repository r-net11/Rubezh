using System.Linq;
using FiresecAPI.Automation;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
namespace AutomationModule.ViewModels
{
	public class ForeachStepViewModel : BaseViewModel, IStepViewModel
	{
		public ForeachArguments ForeachArguments { get; private set; }
		Procedure Procedure { get; set; }

		public ForeachStepViewModel(ForeachArguments foreachArguments, Procedure procedure)
		{
			ForeachArguments = foreachArguments;
			Procedure = procedure;
			UpdateContent();
			var d = XManager.DeviceConfiguration;
		}

		public void UpdateContent()
		{
			ListVariables = new ObservableCollection<VariableViewModel>();
			ItemVariables = new ObservableCollection<VariableViewModel>();
			foreach (var variable in Procedure.Variables.FindAll(x => x.IsList && x.ValueType == ValueType.Object))
			{
				var variableViewModel = new VariableViewModel(variable);
				ListVariables.Add(variableViewModel);
			}
			SelectedListVariable = ListVariables.FirstOrDefault(x => x.Variable.Uid == ForeachArguments.ListVariableUid);
			if (SelectedListVariable != null)
				UpdateItemVariables(SelectedListVariable.ObjectType);
			OnPropertyChanged(() => ListVariables);
			OnPropertyChanged(() => ItemVariables);
		}

		void UpdateItemVariables(ObjectType objectType)
		{
			ItemVariables = new ObservableCollection<VariableViewModel>();
			foreach (var variable in Procedure.Variables.FindAll(x => !x.IsList && x.ValueType == ValueType.Object && x.ObjectType == objectType))
			{
				var variableViewModel = new VariableViewModel(variable);
				ItemVariables.Add(variableViewModel);
			}
			SelectedItemVariable = ItemVariables.FirstOrDefault(x => x.Variable.Uid == ForeachArguments.ItemVariableUid);
			OnPropertyChanged(() => ItemVariables);
		}

		public ObservableCollection<VariableViewModel> ListVariables { get; private set; }
		private VariableViewModel _selectedListVariable;
		public VariableViewModel SelectedListVariable
		{
			get { return _selectedListVariable; }
			set
			{
				_selectedListVariable = value;
				if (value != null)
				{
					ForeachArguments.ListVariableUid = value.Variable.Uid;
					UpdateItemVariables(value.ObjectType);
				}
				OnPropertyChanged(() => SelectedListVariable);
			}
		}
		public ObservableCollection<VariableViewModel> ItemVariables { get; private set; }
		private VariableViewModel _selectedItemVariable;
		public VariableViewModel SelectedItemVariable
		{
			get { return _selectedItemVariable; }
			set
			{
				_selectedItemVariable = value;
				if (value != null)
					ForeachArguments.ItemVariableUid = value.Variable.Uid;
				OnPropertyChanged(() => SelectedItemVariable);
			}
		}

		public string Description
		{
			get { return ""; }
		}

	}
}
