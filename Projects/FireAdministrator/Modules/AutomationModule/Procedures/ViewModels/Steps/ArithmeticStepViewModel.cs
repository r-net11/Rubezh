using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ArithmeticStepViewModel : BaseViewModel
	{
		public ArithmeticArguments ArithmeticArguments { get; private set; }
		public Procedure Procedure { get; private set; }
		public ArithmeticParameterViewModel Variable1 { get; set; }
		public ArithmeticParameterViewModel Variable2 { get; set; }
		public ArithmeticParameterViewModel Result { get; set; }

		public ArithmeticStepViewModel(ProcedureStep procedureStep, Procedure procedure)
		{
			Procedure = procedure;
			ArithmeticArguments = procedureStep.ArithmeticArguments;
			SelectedArithmeticType = ArithmeticArguments.ArithmeticType;
			Variable1 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable1, Procedure.Variables);
			Variable2 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable2, Procedure.Variables);
			Result = new ArithmeticParameterViewModel(ArithmeticArguments.Result, Procedure.Variables, true);
			ArithmeticTypes = new ObservableCollection<ArithmeticType> { ArithmeticType.Add, ArithmeticType.Sub, ArithmeticType.Multi, ArithmeticType.Div };
		}

		public void UpdateContent(List<Variable> localVariables)
		{
			Variable1.Update(localVariables);
			Variable2.Update(localVariables);
			Result.Update(localVariables);
		}

		public ObservableCollection<ArithmeticType> ArithmeticTypes { get; private set; }
		public ArithmeticType SelectedArithmeticType
		{
			get { return ArithmeticArguments.ArithmeticType; }
			set
			{
				ArithmeticArguments.ArithmeticType = value;
				OnPropertyChanged(() => SelectedArithmeticType);
			}
		}
	}

	public class ArithmeticParameterViewModel : BaseViewModel
	{
		public ArithmeticParameter ArithmeticParameter { get; private set; }
		public List<ValueType> ValueTypes { get; private set; }
		public ObservableCollection<VariableViewModel> Variables { get; private set; }
		public ObservableCollection<GlobalVariableViewModel> GlobalVariables { get; private set; }

		public ArithmeticParameterViewModel(ArithmeticParameter arithmeticParameter, List<Variable> localVariables, bool isResult = false)
		{
			ArithmeticParameter = arithmeticParameter;
			ValueTypes = new List<ValueType>();
			if (!isResult)
				ValueTypes.Add(ValueType.IsValue);
			ValueTypes.Add(ValueType.IsGlobalVariable);
			ValueTypes.Add(ValueType.IsLocalVariable);
			Update(localVariables);
		}

		public void Update(List<Variable> localVariables)
		{
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			Variables = new ObservableCollection<VariableViewModel>();
			if (localVariables == null)
				localVariables = new List<Variable>();
			foreach (var variable in localVariables.FindAll(x => x.VariableType == VariableType.Integer))
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}

			if (localVariables.Any(x => x.Uid == ArithmeticParameter.VariableUid))
				SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == ArithmeticParameter.VariableUid);
			else
				SelectedVariable = Variables.FirstOrDefault();

			GlobalVariables = new ObservableCollection<GlobalVariableViewModel>();
			if (FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables == null)
				FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables = new List<GlobalVariable>();
			foreach (var globalVariable in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables)
			{
				var globalVariableViewModel = new GlobalVariableViewModel(globalVariable);
				GlobalVariables.Add(globalVariableViewModel);
			}
			if (GlobalVariables.Any(x => x.GlobalVariable.Uid == ArithmeticParameter.GlobalVariableUid))
				SelectedGlobalVariable = GlobalVariables.FirstOrDefault(x => x.GlobalVariable.Uid == ArithmeticParameter.GlobalVariableUid);
			else
				SelectedGlobalVariable = GlobalVariables.FirstOrDefault();
			SelectedValueType = ValueTypes.FirstOrDefault();
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			OnPropertyChanged(() => GlobalVariables);
			OnPropertyChanged(() => Variables);
		}

		public ValueType SelectedValueType
		{
			get { return ArithmeticParameter.ValueType; }
			set
			{
				ArithmeticParameter.ValueType = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedValueType);
			}
		}

		public int Value
		{
			get { return ArithmeticParameter.Value; }
			set
			{
				ArithmeticParameter.Value = value;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => Value);
			}
		}

		GlobalVariableViewModel _selectedGlobalVariable;
		public GlobalVariableViewModel SelectedGlobalVariable
		{
			get { return _selectedGlobalVariable; }
			set
			{
				_selectedGlobalVariable = value;
				if (_selectedGlobalVariable != null)
					ArithmeticParameter.GlobalVariableUid = value.GlobalVariable.Uid;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedGlobalVariable);
			}
		}

		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				if (_selectedVariable != null)
					ArithmeticParameter.VariableUid = value.Variable.Uid;
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariable);
			}
		}
	}
}