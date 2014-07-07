using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class FindObjectStepViewModel : BaseViewModel, IStepViewModel
	{
		FindObjectArguments FindObjectArguments { get; set; }
		Procedure Procedure { get; set; }
		public ObservableCollection<FindObjectConditionViewModel> FindObjectConditions { get; private set; }

		public FindObjectStepViewModel(FindObjectArguments findObjectArguments, Procedure procedure)
		{
			FindObjectArguments = findObjectArguments;
			Procedure = procedure;
			UpdateContent();
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<FindObjectConditionViewModel>(OnRemove);
			ChangeJoinOperatorCommand = new RelayCommand(OnChangeJoinOperator);
			RunCommand = new RelayCommand(OnRun);
		}

		public RelayCommand AddCommand { get; private set; }
		public void OnAdd()
		{
			var findObjectCondition = new FindObjectCondition();
			var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition, SelectedVariable.Variable);
			FindObjectArguments.FindObjectConditions.Add(findObjectCondition);
			FindObjectConditions.Add(findObjectConditionViewModel);
			OnPropertyChanged(() => FindObjectConditions);
		}

		public RelayCommand<FindObjectConditionViewModel> RemoveCommand { get; private set; }
		void OnRemove(FindObjectConditionViewModel findObjectConditionViewModel)
		{
			FindObjectConditions.Remove(findObjectConditionViewModel);
			FindObjectArguments.FindObjectConditions.Remove(findObjectConditionViewModel.FindObjectCondition);
			OnPropertyChanged(() => FindObjectConditions);
			ServiceFactory.SaveService.AutomationChanged = true;
		}

		public RelayCommand ChangeJoinOperatorCommand { get; private set; }
		void OnChangeJoinOperator()
		{
			JoinOperator = JoinOperator == JoinOperator.And ? JoinOperator.Or : JoinOperator.And;
		}

		public void UpdateContent()
		{
			Variables = new ObservableCollection<VariableViewModel>();
			foreach (var variable in Procedure.Variables.FindAll(x => ((x.VariableType == VariableType.Object) && (x.IsList))))
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == FindObjectArguments.ResultUid);
			JoinOperator = FindObjectArguments.JoinOperator;
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			FindObjectConditions = new ObservableCollection<FindObjectConditionViewModel>();
			if (SelectedVariable != null)
				foreach (var findObjectCondition in FindObjectArguments.FindObjectConditions)
				{
					var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition, SelectedVariable.Variable);
					FindObjectConditions.Add(findObjectConditionViewModel);
				}
			else
				FindObjectArguments.FindObjectConditions = new List<FindObjectCondition>();
			OnPropertyChanged(() => Variables);
			OnPropertyChanged(() => SelectedVariable);
			OnPropertyChanged(() => FindObjectConditions);
		}

		public string Description
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
				ServiceFactory.SaveService.AutomationChanged = true;
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
					FindObjectArguments.ResultUid = value.Variable.Uid;
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariable);
			}
		}

		public RelayCommand RunCommand { get; private set; }
		private void OnRun()
		{
			
			//FiresecManager.SystemConfiguration.AutomationConfiguration.UpdateConfiguration();
			FiresecManager.FiresecService.RunProcedure(Procedure.Uid, new List<Argument>());
		}
	}

	public class FindObjectConditionViewModel : BaseViewModel
	{
		public FindObjectCondition FindObjectCondition { get; private set; }
		Variable ResultVariable { get; set; }

		public FindObjectConditionViewModel(FindObjectCondition findObjectCondition, Variable resultVariable)
		{
			FindObjectCondition = findObjectCondition;
			ResultVariable = resultVariable;
			DevicePropertyTypes = new ObservableCollection<DevicePropertyType>
			{
				DevicePropertyType.Name, DevicePropertyType.ShleifNo, DevicePropertyType.IntAddress, DevicePropertyType.DeviceState
			};
			ZonePropertyTypes = new ObservableCollection<ZonePropertyType>
			{
				ZonePropertyType.Name, ZonePropertyType.No, ZonePropertyType.ZoneType
			};

			DirectionPropertyTypes = new ObservableCollection<DirectionPropertyType>
			{
				DirectionPropertyType.Name, DirectionPropertyType.No, DirectionPropertyType.Delay, DirectionPropertyType.Hold, DirectionPropertyType.DelayRegime
			};
			ConditionTypes = new ObservableCollection<ConditionType>
			{
				ConditionType.IsEqual, ConditionType.IsNotEqual, ConditionType.IsMore, ConditionType.IsNotMore, ConditionType.IsLess, ConditionType.IsNotLess
			};
			StringConditionTypes = new ObservableCollection<StringConditionType>
			{
				StringConditionType.StartsWith, StringConditionType.EndsWith, StringConditionType.Contains
			};

			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			SelectedDevicePropertyType = FindObjectCondition.DevicePropertyType;
			SelectedZonePropertyType = FindObjectCondition.ZonePropertyType;
			SelectedDirectionPropertyType = FindObjectCondition.DirectionPropertyType;
			SelectedConditionType = FindObjectCondition.ConditionType;
			IntValue = FindObjectCondition.IntValue;
			StringValue = FindObjectCondition.StringValue;
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
		}

		public ObservableCollection<DevicePropertyType> DevicePropertyTypes { get; private set; }
		public DevicePropertyType SelectedDevicePropertyType
		{
			get { return FindObjectCondition.DevicePropertyType; }
			set
			{
				FindObjectCondition.DevicePropertyType = value;
				OnPropertyChanged(() => SelectedDevicePropertyType);
				OnPropertyChanged(() => PropertyType);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public ObservableCollection<ZonePropertyType> ZonePropertyTypes { get; private set; }
		public ZonePropertyType SelectedZonePropertyType
		{
			get { return FindObjectCondition.ZonePropertyType; }
			set
			{
				FindObjectCondition.ZonePropertyType = value;
				OnPropertyChanged(() => SelectedZonePropertyType);
				OnPropertyChanged(() => PropertyType);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public ObservableCollection<DirectionPropertyType> DirectionPropertyTypes { get; private set; }
		public DirectionPropertyType SelectedDirectionPropertyType
		{
			get { return FindObjectCondition.DirectionPropertyType; }
			set
			{
				FindObjectCondition.DirectionPropertyType = value;
				OnPropertyChanged(() => SelectedDirectionPropertyType);
				OnPropertyChanged(() => PropertyType);
				ServiceFactory.SaveService.AutomationChanged = true;
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
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public ObservableCollection<StringConditionType> StringConditionTypes { get; private set; }
		public StringConditionType SelectedStringConditionType
		{
			get { return FindObjectCondition.StringConditionType; }
			set
			{
				FindObjectCondition.StringConditionType = value;
				OnPropertyChanged(() => SelectedStringConditionType);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public int IntValue
		{
			get { return FindObjectCondition.IntValue; }
			set
			{
				FindObjectCondition.IntValue = value;
				OnPropertyChanged(() => IntValue);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public string StringValue
		{
			get { return FindObjectCondition.StringValue; }
			set
			{
				FindObjectCondition.StringValue = value;
				OnPropertyChanged(() => StringValue);
				ServiceFactory.SaveService.AutomationChanged = true;
			}
		}

		public PropertyType PropertyType
		{
			get
			{
				if (ResultVariable.ObjectType == ObjectType.Device)
				{
					switch (SelectedDevicePropertyType)
					{
						case DevicePropertyType.ShleifNo:
						case DevicePropertyType.IntAddress:
						case DevicePropertyType.DeviceState:
							return FindObjectCondition.PropertyType = PropertyType.Integer;
						case DevicePropertyType.Name:
							return FindObjectCondition.PropertyType = PropertyType.String;
					}
				}

				if (ResultVariable.ObjectType == ObjectType.Zone)
				{
					switch (SelectedZonePropertyType)
					{
						case ZonePropertyType.No:
						case ZonePropertyType.ZoneType:
							return FindObjectCondition.PropertyType = PropertyType.Integer;
						case ZonePropertyType.Name:
							return FindObjectCondition.PropertyType = PropertyType.String;
					}
				}

				if (ResultVariable.ObjectType == ObjectType.Direction)
				{
					switch (SelectedDirectionPropertyType)
					{
						case DirectionPropertyType.No:
						case DirectionPropertyType.Delay:
						case DirectionPropertyType.Hold:
						case DirectionPropertyType.DelayRegime:
							return FindObjectCondition.PropertyType = PropertyType.Integer;
						case DirectionPropertyType.Name:
							return FindObjectCondition.PropertyType = PropertyType.String;
					}
				}
				return PropertyType.String;
			}
		}
	}
}
