using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.GK;
using Infrastructure.Common;

namespace AutomationModule.ViewModels
{
	public class ArithmeticParameterViewModel : BaseViewModel
	{
		public ArithmeticParameter ArithmeticParameter { get; private set; }
		public VariableItemViewModel CurrentVariableItem { get; private set; }
		public Action UpdateVariableScopeHandler { get; set; }
		public Action UpdateVariableHandler { get; set; }
		Action UpdateDescriptionHandler { get; set; }

		public ArithmeticParameterViewModel(ArithmeticParameter arithmeticParameter, Action updateDescriptionHandler, bool allowImplicitValue = true)
		{
			UpdateDescriptionHandler = updateDescriptionHandler;
			CurrentVariableItem = new VariableItemViewModel(arithmeticParameter.VariableItem);
			CurrentVariableItem.UpdateDescriptionHandler = updateDescriptionHandler;
			Variables = new List<VariableViewModel>();
			EnumTypes = ProcedureHelper.GetEnumObs<EnumType>();
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			ExplicitTypes = ProcedureHelper.GetEnumObs<ExplicitType>();
			var availableVariableScopes = ProcedureHelper.GetEnumList<VariableScope>().FindAll(x => allowImplicitValue || x != VariableScope.ExplicitValue);
			ArithmeticParameter = arithmeticParameter;
			VariableScopes = new ObservableCollection<VariableScope>(availableVariableScopes);
			OnPropertyChanged(() => VariableScopes);
			ChangeItemCommand = new RelayCommand(OnChangeItem);
		}

		public RelayCommand ChangeItemCommand { get; private set; }
		void OnChangeItem()
		{
			ProcedureHelper.SelectObject(ObjectType, CurrentVariableItem);
			if (UpdateVariableScopeHandler != null)
				UpdateVariableScopeHandler();
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

		public ObservableCollection<ExplicitType> ExplicitTypes { get; private set; }
		public ExplicitType ExplicitType
		{
			get { return ArithmeticParameter.ExplicitType; }
			set
			{
				ArithmeticParameter.ExplicitType = value;
				OnPropertyChanged(() => ExplicitType);
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

		public void Update(List<Variable> variables)
		{
			Variables = new List<VariableViewModel>();
			foreach (var variable in variables)
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == ArithmeticParameter.VariableUid);
			SelectedVariableScope = ArithmeticParameter.VariableScope;
			OnPropertyChanged(() => LocalVariables);
			OnPropertyChanged(() => GlobalVariables);
		}

		public ObservableCollection<VariableScope> VariableScopes { get; set; }
		public VariableScope SelectedVariableScope
		{
			get { return ArithmeticParameter.VariableScope; }
			set
			{
				ArithmeticParameter.VariableScope = value;
				if (value == VariableScope.ExplicitValue)
					SelectedVariable = null;
				if (value == VariableScope.LocalVariable)
					SelectedVariable = LocalVariables.FirstOrDefault(x => x.Variable.Uid == ArithmeticParameter.VariableUid);
				if (value == VariableScope.GlobalVariable)
					SelectedVariable = GlobalVariables.FirstOrDefault(x => x.Variable.Uid == ArithmeticParameter.VariableUid);
				if (UpdateVariableScopeHandler != null)
					UpdateVariableScopeHandler();
				OnPropertyChanged(() => SelectedVariableScope);
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
					ObjectType = value.ObjectType;
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
				if (SelectedVariableScope != VariableScope.ExplicitValue)
				{
					if ((SelectedVariable == null) || (SelectedVariable.Variable.IsGlobal && SelectedVariableScope == VariableScope.LocalVariable)
						|| (!SelectedVariable.Variable.IsGlobal && SelectedVariableScope == VariableScope.GlobalVariable))
						return "<пусто>";
					else
						return "<" + SelectedVariable.Name + ">";
				}

				var description = "";
				switch (ExplicitType)
				{
					case ExplicitType.Boolean:
						description = CurrentVariableItem.BoolValue.ToString();
						break;
					case ExplicitType.DateTime:
						description = CurrentVariableItem.DateTimeValue.ToString();
						break;
					case ExplicitType.Integer:
						description = CurrentVariableItem.IntValue.ToString();
						break;
					case ExplicitType.String:
						description = CurrentVariableItem.StringValue.ToString();
						break;
					case ExplicitType.Enum:
						{
							if (EnumType == EnumType.StateType)
								description = CurrentVariableItem.StateTypeValue.ToDescription();
							if (EnumType == EnumType.DriverType)
								description = CurrentVariableItem.DriverTypeValue.ToDescription();
						}
						break;
					case ExplicitType.Object:
						{
							description = CurrentVariableItem.PresentationName;
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