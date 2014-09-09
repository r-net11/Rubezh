using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;
using FiresecAPI.GK;
using System.Reflection.Emit;

namespace AutomationModule.ViewModels
{
	public class FindObjectStepViewModel : BaseStepViewModel
	{
		FindObjectArguments FindObjectArguments { get; set; }
		Procedure Procedure { get; set; }
		public ObservableCollection<FindObjectConditionViewModel> FindObjectConditions { get; private set; }

		public FindObjectStepViewModel(FindObjectArguments findObjectArguments, Procedure procedure, Action updateDescriptionHandler)
			: base(updateDescriptionHandler)
		{
			FindObjectArguments = findObjectArguments;
			Procedure = procedure;
			UpdateContent();
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<FindObjectConditionViewModel>(OnRemove);
			ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);
		}

		public RelayCommand AddCommand { get; private set; }
		public void OnAdd()
		{
			var findObjectCondition = new FindObjectCondition();
			var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition, Procedure);
			FindObjectArguments.FindObjectConditions.Add(findObjectCondition);
			FindObjectConditions.Add(findObjectConditionViewModel);			
			OnPropertyChanged(() => FindObjectConditions);
			UpdateContent();
		}

		public RelayCommand<FindObjectConditionViewModel> RemoveCommand { get; private set; }
		void OnRemove(FindObjectConditionViewModel findObjectConditionViewModel)
		{
			FindObjectConditions.Remove(findObjectConditionViewModel);
			FindObjectArguments.FindObjectConditions.Remove(findObjectConditionViewModel.FindObjectCondition);
			OnPropertyChanged(() => FindObjectConditions);
			UpdateContent();
		}

		public RelayCommand ChangeJoinOperatorCommand { get; private set; }
		void OnChangeJoinOperator()
		{
			JoinOperator = JoinOperator == JoinOperator.And ? JoinOperator.Or : JoinOperator.And;
		}

		public bool IsJoinOperatorVisible
		{
			get { return FindObjectConditions.Count > 1; }
		}

		public override void UpdateContent()
		{
			var allVariables = ProcedureHelper.GetAllVariables(Procedure);
			Variables = new ObservableCollection<VariableViewModel>();
			foreach (var variable in allVariables.FindAll(x => ((x.ValueType == ValueType.Object) && (x.IsList))))
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == FindObjectArguments.ResultUid);
			JoinOperator = FindObjectArguments.JoinOperator;
			FindObjectConditions = new ObservableCollection<FindObjectConditionViewModel>();
			if (SelectedVariable != null)
				foreach (var findObjectCondition in FindObjectArguments.FindObjectConditions)
				{
					var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition, Procedure);
					FindObjectConditions.Add(findObjectConditionViewModel);
				}
			else
				FindObjectArguments.FindObjectConditions = new List<FindObjectCondition>();
			OnPropertyChanged(() => Variables);
			OnPropertyChanged(() => SelectedVariable);
			OnPropertyChanged(() => FindObjectConditions);
			OnPropertyChanged(() => IsJoinOperatorVisible);
		}

		public override string Description
		{
			get { return ""; }
		}

		public JoinOperator JoinOperator
		{
			get { return FindObjectArguments.JoinOperator; }
			set
			{
				FindObjectArguments.JoinOperator = value;
				OnPropertyChanged(() => JoinOperator);
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
				if (value != null)
				{
					if (FindObjectArguments.ResultUid != value.Variable.Uid)
					{
						FindObjectConditions = new ObservableCollection<FindObjectConditionViewModel>();
						FindObjectArguments.FindObjectConditions = new List<FindObjectCondition>();
						OnPropertyChanged(() => FindObjectConditions);
					}
					FindObjectConditionViewModel.Properties = new ObservableCollection<Property>(ProcedureHelper.ObjectTypeToProperiesList(value.Variable.ObjectType));
					FindObjectArguments.ResultUid = value.Variable.Uid;
				}
				OnPropertyChanged(() => SelectedVariable);
			}
		}
	}

	public class FindObjectConditionViewModel : BaseViewModel
	{
		public FindObjectCondition FindObjectCondition { get; private set; }
		public ArithmeticParameterViewModel Variable2 { get; private set; }
		Procedure Procedure { get; set; }

		public FindObjectConditionViewModel(FindObjectCondition findObjectCondition, Procedure procedure)
		{
			FindObjectCondition = findObjectCondition;
			Procedure = procedure;
			Variable2 = new ArithmeticParameterViewModel(findObjectCondition.Variable2);
			SelectedProperty = FindObjectCondition.Property;			
			SelectedConditionType = FindObjectCondition.ConditionType;
		}

		public static ObservableCollection<Property> Properties { get; set; }
		public Property SelectedProperty
		{
			get { return FindObjectCondition.Property; }
			set
			{							
				FindObjectCondition.Property = value;
				if (value == Property.IntAddress)
				{ MinValue = 1; MaxValue = 255; }
				if (value == Property.ShleifNo)
				{ MinValue = 1; MaxValue = 8; }
				ConditionTypes = new ObservableCollection<ConditionType>(ProcedureHelper.ObjectTypeToConditionTypesList(ValueType));
				Variable2.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => !x.IsList && x.ValueType == ValueType && x.EnumType == EnumType));
				Variable2.ValueType = ValueType;
				Variable2.EnumType = EnumType;
				OnPropertyChanged(() => SelectedProperty);
				OnPropertyChanged(() => ConditionTypes);
				OnPropertyChanged(() => MinValue);
				OnPropertyChanged(() => MaxValue);
			}
		}

		public ObservableCollection<ConditionType> ConditionTypes { get; private set; }
		public ConditionType SelectedConditionType
		{
			get { return FindObjectCondition.ConditionType; }
			set
			{
				FindObjectCondition.ConditionType = value;
				OnPropertyChanged(() => SelectedConditionType);
			}
		}

		int _minValue;
		public int MinValue
		{
			get { return _minValue; }
			set
			{
				_minValue = value;
				OnPropertyChanged(() => MinValue);
			}
		}

		int _maxValue;
		public int MaxValue
		{ 
			get { return _maxValue; }
			set
			{
				_maxValue = value;
				OnPropertyChanged(() => MaxValue);
			}
		}

		public EnumType EnumType
		{
			get
			{
				if (SelectedProperty == Property.Type)
					return EnumType.DeviceType;
				if (SelectedProperty == Property.DeviceState)
					return EnumType.StateClass;
				return EnumType.StateClass;
			}
		}

		public ValueType ValueType
		{
			get
			{
				if (SelectedProperty == Property.Description)
					return ValueType.String;
				if ((SelectedProperty == Property.Type)||(SelectedProperty == Property.DeviceState))
					return ValueType.Enum;
				return ValueType.Integer;
			}
		}
	}
}
