using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using FiresecAPI.Automation;
using FiresecClient;

namespace AutomationModule.ViewModels
{
	public class ArgumentItemViewModel : BaseViewModel
	{
		Procedure Procedure { get; set; }
		ArithmeticParameter ArithmeticParameter { get; set; }
		List<FiresecAPI.Automation.ValueType> ValueTypes { get; set; }

		public Action UpdateDescriptionHandler { get; set; }
		public Action UpdateVariableTypeHandler { get; set; }
		public Action UpdateVariableHandler { get; set; }

		public ArgumentItemViewModel(Procedure procedure, ArithmeticParameter arithmeticParameter, List<FiresecAPI.Automation.ValueType> valueTypes, bool allowImplicitValue = true)
		{
			ArithmeticParameter = arithmeticParameter;
			Procedure = procedure;
			ValueTypes = valueTypes;

			VariableTypes = new ObservableCollection<VariableType>();
			VariableTypes.Add(VariableType.IsGlobalVariable);
			VariableTypes.Add(VariableType.IsLocalVariable);
			if (allowImplicitValue)
			{
				VariableTypes.Add(VariableType.IsValue);
			}

			SelectedVariableType = ArithmeticParameter.VariableType;
		}

		public void Update()
		{
			//UpdateVariables();
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == ArithmeticParameter.VariableUid);
			if (SelectedVariable != null)
			{
				SelectedVariable.Update();
			}
		}

		public ObservableCollection<VariableType> VariableTypes { get; private set; }

		public VariableType SelectedVariableType
		{
			get { return ArithmeticParameter.VariableType; }
			set
			{
				ArithmeticParameter.VariableType = value;
				if (UpdateVariableTypeHandler != null)
					UpdateVariableTypeHandler();
				OnPropertyChanged(() => SelectedVariableType);
				UpdateVariables();	
				ShowVariables = value != VariableType.IsValue;
			}
		}

		void UpdateVariables()
		{
			Variables = new ObservableCollection<VariableViewModel>();
			switch (SelectedVariableType)
			{
				case VariableType.IsLocalVariable:
					foreach (var argumrnt in Procedure.Arguments)
					{
						if (ValueTypes.Contains(argumrnt.ValueType))
						{
							var variableViewModel = new VariableViewModel(argumrnt);
							Variables.Add(variableViewModel);
						}
					}
					foreach (var variable in Procedure.Variables)
					{
						if (ValueTypes.Contains(variable.ValueType))
						{
							var variableViewModel = new VariableViewModel(variable);
							Variables.Add(variableViewModel);
						}
					}
					break;

				case VariableType.IsGlobalVariable:
					foreach (var variable in FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables)
					{
						if (ValueTypes.Contains(variable.ValueType))
						{
							var variableViewModel = new VariableViewModel(variable);
							Variables.Add(variableViewModel);
						}
					}
					break;
			}
		}

		ObservableCollection<VariableViewModel> _variables;
		public ObservableCollection<VariableViewModel> Variables
		{
			get { return _variables; }
			set
			{
				_variables = value;
				OnPropertyChanged(() => Variables);
			}
		}

		bool _showVariables;
		public bool ShowVariables
		{
			get { return _showVariables; }
			set
			{
				_showVariables = value;
				OnPropertyChanged(() => ShowVariables);
			}
		}

		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				if (value != null)
				{
					ArithmeticParameter.VariableUid = value.Variable.Uid;
					if (UpdateVariableHandler != null)
						UpdateVariableHandler();
				}
				else
				{
					ArithmeticParameter.VariableUid = Guid.Empty;
				}
				OnPropertyChanged(() => SelectedVariable);
			}
		}

		public bool BoolValue
		{
			get { return ArithmeticParameter.VariableItem.BoolValue; }
			set
			{
				ArithmeticParameter.VariableItem.BoolValue = value;
				OnPropertyChanged(() => BoolValue);
			}
		}

		public DateTime DateTimeValue
		{
			get { return ArithmeticParameter.VariableItem.DateTimeValue; }
			set
			{
				ArithmeticParameter.VariableItem.DateTimeValue = value;
				OnPropertyChanged(() => DateTimeValue);
			}
		}

		public int IntValue
		{
			get { return ArithmeticParameter.VariableItem.IntValue; }
			set
			{
				ArithmeticParameter.VariableItem.IntValue = value;
				OnPropertyChanged(() => IntValue);
			}
		}

		public string StringValue
		{
			get { return ArithmeticParameter.VariableItem.StringValue; }
			set
			{
				ArithmeticParameter.VariableItem.StringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}

		public Guid UIDValue
		{
			get { return ArithmeticParameter.VariableItem.UidValue; }
			set
			{
				ArithmeticParameter.VariableItem.UidValue = value;
				OnPropertyChanged(() => UIDValue);
			}
		}

		public new void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			ServiceFactory.SaveService.AutomationChanged = true;
			base.OnPropertyChanged(propertyExpression);
			if (UpdateDescriptionHandler != null)
				UpdateDescriptionHandler();
		}
	}
}