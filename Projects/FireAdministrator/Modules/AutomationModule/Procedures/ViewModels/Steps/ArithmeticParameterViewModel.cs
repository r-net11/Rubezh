using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using ValueType = FiresecAPI.Automation.ValueType;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.GK;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class ArithmeticParameterViewModel : BaseViewModel
	{
		public ArithmeticParameter ArithmeticParameter { get; private set; }
		public Action UpdateVariableTypeHandler { get; set; }
		public Action UpdateVariableHandler { get; set; }
		public VariableItemViewModel CurrentVariableItem { get; private set; }

		public ArithmeticParameterViewModel(ArithmeticParameter arithmeticParameter, bool allowImplicitValue = true)
		{
			CurrentVariableItem = new VariableItemViewModel(arithmeticParameter.VariableItem);
			Variables = new List<VariableViewModel>();
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>();
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			ValueTypes = ProcedureHelper.GetEnumObs<ValueType>();
			var availableVariableTypes = ProcedureHelper.GetEnumList<VariableType>().FindAll(x => allowImplicitValue || x != VariableType.IsValue);
			ArithmeticParameter = arithmeticParameter;
			VariableTypes = new ObservableCollection<VariableType>(availableVariableTypes);
			OnPropertyChanged(() => VariableTypes);
			ChangeItemCommand = new RelayCommand(OnChangeItem);
		}

		public RelayCommand ChangeItemCommand { get; private set; }
		void OnChangeItem()
		{
			CurrentVariableItem = ProcedureHelper.SelectObject(ObjectType, CurrentVariableItem);
			UpdateVariableTypeHandler();
			OnPropertyChanged(() => CurrentVariableItem);
		}

		public ObservableCollection<EnumType> EnumTypes { get; private set; }
		public EnumType EnumType
		{
			get { return ArithmeticParameter.EnumType; }
			set
			{
				ArithmeticParameter.EnumType = value;
				OnPropertyChanged(() => EnumType);
			}
		}

		public ObservableCollection<ValueType> ValueTypes { get; private set; }
		public ValueType ValueType
		{
			get { return ArithmeticParameter.ValueType; }
			set
			{
				ArithmeticParameter.ValueType = value;
				OnPropertyChanged(() => ValueType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		public ObjectType ObjectType
		{
			get { return ArithmeticParameter.ObjectType; }
			set
			{
				ArithmeticParameter.ObjectType = value;
				OnPropertyChanged(() => ObjectType);
			}
		}

		Action _updateDescriptionHandler;
		public Action UpdateDescriptionHandler
		{
			get { return _updateDescriptionHandler; }
			set
			{
				_updateDescriptionHandler = value;
				CurrentVariableItem.UpdateDescriptionHandler = value;
			}
		}

		public void Update(List<Variable> variables)
		{
			Variables = new List<VariableViewModel>();
			foreach (var variable in variables)
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == ArithmeticParameter.VariableUid);
			SelectedVariableType = ArithmeticParameter.VariableType;
			OnPropertyChanged(() => LocalVariables);
			OnPropertyChanged(() => GlobalVariables);
		}

		public ObservableCollection<VariableType> VariableTypes { get; set; }
		public VariableType SelectedVariableType
		{
			get { return ArithmeticParameter.VariableType; }
			set
			{
				ArithmeticParameter.VariableType = value;
				if (UpdateVariableTypeHandler != null)
					UpdateVariableTypeHandler();
				OnPropertyChanged(() => SelectedVariableType);
			}
		}

		List<VariableViewModel> Variables { get; set; }
		public ObservableCollection<VariableViewModel> LocalVariables
		{
			get { return new ObservableCollection<VariableViewModel>(Variables.FindAll(x => !x.Variable.IsGlobal)); }
		}

		public ObservableCollection<VariableViewModel> GlobalVariables
		{
			get { return new ObservableCollection<VariableViewModel>(Variables.FindAll(x => x.Variable.IsGlobal)); }
		}

		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				if (_selectedVariable != null)
				{
					ArithmeticParameter.VariableUid = value.Variable.Uid;
					EnumType = value.EnumType;
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

		public string Description
		{
			get
			{
				if (SelectedVariableType != VariableType.IsValue)
				{
					if ((SelectedVariable == null) || (SelectedVariable.Variable.IsGlobal && SelectedVariableType == VariableType.IsLocalVariable)
						|| (!SelectedVariable.Variable.IsGlobal && SelectedVariableType == VariableType.IsGlobalVariable))
						return "пусто";
					else
						return SelectedVariable.Name;
				}

				var description = "";
				switch (ValueType)
				{
					case ValueType.Boolean:
						description = CurrentVariableItem.VariableItem.BoolValue.ToString();
						break;
					case ValueType.DateTime:
						description = CurrentVariableItem.VariableItem.DateTimeValue.ToString();
						break;
					case ValueType.Integer:
						description = CurrentVariableItem.VariableItem.IntValue.ToString();
						break;
					case ValueType.String:
						description = CurrentVariableItem.VariableItem.StringValue.ToString();
						break;
					case ValueType.Enum:
						{
							if (EnumType == EnumType.StateClass)
								description = CurrentVariableItem.VariableItem.StateClassValue.ToDescription();
							if (EnumType == EnumType.DeviceType)
								description = CurrentVariableItem.VariableItem.DeviceType;
						}
						break;
				}
				return description;
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