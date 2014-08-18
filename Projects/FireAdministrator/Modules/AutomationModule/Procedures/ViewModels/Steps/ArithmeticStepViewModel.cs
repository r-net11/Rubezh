using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.ViewModels
{
	public class ArithmeticStepViewModel : BaseViewModel, IStepViewModel
	{
		public ArithmeticArguments ArithmeticArguments { get; private set; }
		public Procedure Procedure { get; private set; }
		public ArithmeticParameterViewModel Variable1 { get; set; }
		public ArithmeticParameterViewModel Variable2 { get; set; }
		public ArithmeticParameterViewModel Result { get; set; }
		public Action UpdateDescriptionHandler { get; set; }
		public ArithmeticStepViewModel(ArithmeticArguments arithmeticArguments, Procedure procedure, Action updateDescriptionHandler)
		{
			Procedure = procedure;
			UpdateDescriptionHandler = updateDescriptionHandler;
			ArithmeticArguments = arithmeticArguments;
			SelectedArithmeticType = ArithmeticArguments.ArithmeticType;
			var variablesAndArguments = new List<Variable>(Procedure.Variables);
			variablesAndArguments.AddRange(Procedure.Arguments);
			Variable1 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable1, variablesAndArguments);
			Variable2 = new ArithmeticParameterViewModel(ArithmeticArguments.Variable2, variablesAndArguments);
			Result = new ArithmeticParameterViewModel(ArithmeticArguments.Result, variablesAndArguments, true);
			Variable1.UpdateDescriptionHandler = updateDescriptionHandler;
			Variable2.UpdateDescriptionHandler = updateDescriptionHandler;
			Result.UpdateDescriptionHandler = updateDescriptionHandler;
			ArithmeticTypes = new ObservableCollection<ArithmeticType> { ArithmeticType.Add, ArithmeticType.Sub, ArithmeticType.Multi, ArithmeticType.Div };
		}

		public void UpdateContent()
		{
			var variablesAndArguments = new List<Variable>(Procedure.Variables);
			variablesAndArguments.AddRange(Procedure.Arguments);
			Variable1.Update(variablesAndArguments);
			Variable2.Update(variablesAndArguments);
			Result.Update(variablesAndArguments);
		}

		public string Description
		{
			get
			{
				string var1 = Variable1.DescriptionValue;
				if (String.IsNullOrEmpty(var1))
					var1 = "пусто";
				string var2 = Variable2.DescriptionValue;
				if (String.IsNullOrEmpty(var2))
					var2 = "пусто";
				string res = Result.DescriptionValue;
				if (String.IsNullOrEmpty(res))
					res = "пусто";
				var op = "";
				switch (SelectedArithmeticType)
				{
					case ArithmeticType.Add:
						op = "+";
						break;
					case ArithmeticType.Sub:
						op = "-";
						break;
					case ArithmeticType.Div:
						op = ":";
						break;
					case ArithmeticType.Multi:
						op = "*";
						break;
				}

				return "<" + res + ">" + " = " + "<" + var1 + ">" + op + " " + "<" + var2 + ">";
			}
		}

		public ObservableCollection<ArithmeticType> ArithmeticTypes { get; private set; }
		public ArithmeticType SelectedArithmeticType
		{
			get { return ArithmeticArguments.ArithmeticType; }
			set
			{
				ArithmeticArguments.ArithmeticType = value;
				if (UpdateDescriptionHandler!=null)
					UpdateDescriptionHandler();
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedArithmeticType);
			}
		}
	}

	public class ArithmeticParameterViewModel : BaseViewModel
	{
		public ArithmeticParameter ArithmeticParameter { get; private set; }
		public Action UpdateDescriptionHandler { get; set; }
		public ArithmeticParameterViewModel(ArithmeticParameter arithmeticParameter, List<Variable> localVariables, bool isResult = false)
		{
			ArithmeticParameter = arithmeticParameter;
			VariableTypes = new ObservableCollection<VariableType>();
			VariableTypes.Add(VariableType.IsGlobalVariable);
			VariableTypes.Add(VariableType.IsLocalVariable);
			if (!isResult)
				VariableTypes.Add(VariableType.IsValue);
			Update(localVariables);
		}

		public void Update(List<Variable> localVariables)
		{
			Variables = new ObservableCollection<VariableViewModel>();
			if (localVariables == null)
				localVariables = new List<Variable>();
			foreach (var variable in localVariables.FindAll(x => (x.ValueType != ValueType.Object) && (!x.IsList)))
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}

			if (localVariables.Any(x => x.Uid == ArithmeticParameter.VariableUid))
				SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == ArithmeticParameter.VariableUid);
			else
				SelectedVariable = null;
			GlobalVariables = new ObservableCollection<GlobalVariableViewModel>();
			foreach (var globalVariable in FiresecClient.FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables)
			{
				var globalVariableViewModel = new GlobalVariableViewModel(globalVariable);
				GlobalVariables.Add(globalVariableViewModel);
			}
			if (GlobalVariables.Any(x => x.GlobalVariable.Uid == ArithmeticParameter.GlobalVariableUid))
				SelectedGlobalVariable = GlobalVariables.FirstOrDefault(x => x.GlobalVariable.Uid == ArithmeticParameter.GlobalVariableUid);
			else
				SelectedGlobalVariable = null;

			SelectedVariableType = ArithmeticParameter.VariableType;
			OnPropertyChanged(() => GlobalVariables);
			OnPropertyChanged(() => Variables);
		}

		public ObservableCollection<VariableType> VariableTypes { get; private set; }
		public VariableType SelectedVariableType
		{
			get { return ArithmeticParameter.VariableType; }
			set
			{
				ArithmeticParameter.VariableType = value;
				if (value == VariableType.IsGlobalVariable)
					DescriptionValue = SelectedGlobalVariable != null ? SelectedGlobalVariable.Name : "";
				if (value == VariableType.IsLocalVariable)
					DescriptionValue = SelectedVariable != null ? SelectedVariable.Name : "";
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariableType);
			}
		}

		string _desctriptionValue;
		public string DescriptionValue 
		{
			get { return _desctriptionValue; }
			private set
			{
				_desctriptionValue = value;
				if (UpdateDescriptionHandler != null)
					UpdateDescriptionHandler();
			}
		}

		public int Value
		{
			get { return ArithmeticParameter.Value; }
			set
			{
				ArithmeticParameter.Value = value;
				DescriptionValue = value.ToString();
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => Value);
			}
		}

		public ObservableCollection<GlobalVariableViewModel> GlobalVariables { get; private set; }
		GlobalVariableViewModel _selectedGlobalVariable;
		public GlobalVariableViewModel SelectedGlobalVariable
		{
			get { return _selectedGlobalVariable; }
			set
			{
				_selectedGlobalVariable = value;
				if (_selectedGlobalVariable != null)
				{
					DescriptionValue = value.Name;
					ArithmeticParameter.VariableUid = Guid.Empty;
					ArithmeticParameter.GlobalVariableUid = value.GlobalVariable.Uid;
				}
				else if (SelectedVariableType == VariableType.IsGlobalVariable)
					DescriptionValue = "";
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedGlobalVariable);
			}
		}

		public ObservableCollection<VariableViewModel> Variables { get; private set; }
		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				if (_selectedVariable != null)
				{
					DescriptionValue = value.Name;
					ArithmeticParameter.GlobalVariableUid = Guid.Empty;
					ArithmeticParameter.VariableUid = value.Variable.Uid;
				}
				else if (SelectedVariableType == VariableType.IsLocalVariable)
					DescriptionValue = "";
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariable);
			}
		}
	}
}