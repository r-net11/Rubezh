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
			var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition);
			FindObjectArguments.FindObjectConditions.Add(findObjectCondition);
			FindObjectConditions.Add(findObjectConditionViewModel);
		}

		public RelayCommand<FindObjectConditionViewModel> RemoveCommand { get; private set; }
		void OnRemove(FindObjectConditionViewModel findObjectConditionViewModel)
		{
			FindObjectConditions.Remove(findObjectConditionViewModel);
			FindObjectArguments.FindObjectConditions.Remove(findObjectConditionViewModel.FindObjectCondition);
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
			foreach (var variable in Procedure.Variables.FindAll(x => ((x.VariableType == VariableType.Object) && (x.IsList == true))))
			{
				var variableViewModel = new VariableViewModel(variable);
				Variables.Add(variableViewModel);
			}
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			SelectedVariable = Variables.FirstOrDefault(x => x.Variable.Uid == FindObjectArguments.ResultUid);
			JoinOperator = FindObjectArguments.JoinOperator;
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
			FindObjectConditions = new ObservableCollection<FindObjectConditionViewModel>();
			foreach (var findObjectCondition in FindObjectArguments.FindObjectConditions)
			{
				var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition);
				FindObjectConditions.Add(findObjectConditionViewModel);
			}
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
					FindObjectArguments.ResultUid = value.Variable.Uid;
				}
				ServiceFactory.SaveService.AutomationChanged = true;
				OnPropertyChanged(() => SelectedVariable);
			}
		}

		public void FindDeviceObjectsOr(VariableViewModel result, ObservableCollection<FindObjectConditionViewModel> findObjectConditionsViewModel)
		{

						var resultDevices = new List<Device>();
						foreach (var device in FiresecManager.Devices)
						{
							foreach (var findObjectConditionViewModel in findObjectConditionsViewModel)
							{
								var findObjectCondition = findObjectConditionViewModel.FindObjectCondition;
								int propertyValue = 0;
								switch (findObjectCondition.DevicePropertyType)
								{
									case DevicePropertyType.ShleifNo:
										propertyValue = device.ShleifNo;
										break;
									case DevicePropertyType.IntAddress:
										propertyValue = device.IntAddress;
										break;
								}

								if (resultDevices.Contains(device))
									continue;
								if (((findObjectCondition.ConditionType == ConditionType.IsEqual) && (propertyValue == findObjectCondition.IntValue)) ||
									((findObjectCondition.ConditionType == ConditionType.IsNotEqual) && (propertyValue != findObjectCondition.IntValue)) ||
									((findObjectCondition.ConditionType == ConditionType.IsMore) && (propertyValue > findObjectCondition.IntValue)) ||
									((findObjectCondition.ConditionType == ConditionType.IsNotMore) && (propertyValue <= findObjectCondition.IntValue)) ||
									((findObjectCondition.ConditionType == ConditionType.IsLess) && (propertyValue < findObjectCondition.IntValue)) ||
									((findObjectCondition.ConditionType == ConditionType.IsNotLess) && (propertyValue >= findObjectCondition.IntValue)))
									resultDevices.Add(device);
							}
						}

						foreach (var device in resultDevices)
						{
							var procedureObject = new ProcedureObject();
							procedureObject.ObjectUid = device.UID;
							result.Variable.ProcedureObjects.Add(procedureObject);
						}


		}

		public void FindDeviceObjectsAnd(VariableViewModel result, ObservableCollection<FindObjectConditionViewModel> findObjectConditionsViewModel)
		{
			switch (result.ObjectType)
			{
				case ObjectType.Device:
					{
						var resultDevices = FiresecManager.Devices;
						var tempDevices = new List<Device>(resultDevices);
						foreach (var findObjectConditionViewModel in findObjectConditionsViewModel)
						{
							var findObjectCondition = findObjectConditionViewModel.FindObjectCondition;
							foreach (var device in resultDevices)
							{

								int propertyValue = 0;
								switch (findObjectCondition.DevicePropertyType)
								{
									case DevicePropertyType.ShleifNo:
										propertyValue = device.ShleifNo;
										break;
									case DevicePropertyType.IntAddress:
										propertyValue = device.IntAddress;
										break;
								}

								if (((findObjectCondition.ConditionType == ConditionType.IsEqual) && (propertyValue != findObjectCondition.IntValue)) ||
									((findObjectCondition.ConditionType == ConditionType.IsNotEqual) && (propertyValue == findObjectCondition.IntValue)) ||
									((findObjectCondition.ConditionType == ConditionType.IsMore) && (propertyValue <= findObjectCondition.IntValue)) ||
									((findObjectCondition.ConditionType == ConditionType.IsNotMore) && (propertyValue > findObjectCondition.IntValue)) ||
									((findObjectCondition.ConditionType == ConditionType.IsLess) && (propertyValue >= findObjectCondition.IntValue)) ||
									((findObjectCondition.ConditionType == ConditionType.IsNotLess) && (propertyValue < findObjectCondition.IntValue)))
									tempDevices.Remove(device);
							}
							resultDevices = new List<Device>(tempDevices);
						}

						foreach (var device in resultDevices)
						{
							var procedureObject = new ProcedureObject();
							procedureObject.ObjectUid = device.UID;
							result.Variable.ProcedureObjects.Add(procedureObject);
						}
						break;
					}
			}
		}

		public RelayCommand RunCommand { get; private set; }
		private void OnRun()
		{
			switch (SelectedVariable.ObjectType)
			{
				case ObjectType.Device:
				{
					if (JoinOperator == JoinOperator.Or)
						FindDeviceObjectsOr(SelectedVariable, FindObjectConditions);
					else
						FindDeviceObjectsAnd(SelectedVariable, FindObjectConditions);
					break;
				}
			}
		}
	}

	public class FindObjectConditionViewModel : BaseViewModel
	{
		public FindObjectCondition FindObjectCondition { get; private set; }

		public FindObjectConditionViewModel(FindObjectCondition findObjectCondition)
		{
			FindObjectCondition = findObjectCondition;
			DevicePropertyTypes = new ObservableCollection<DevicePropertyType>
			{
				DevicePropertyType.ShleifNo, DevicePropertyType.IntAddress, DevicePropertyType.Name, DevicePropertyType.DeviceState
			};
			ConditionTypes = new ObservableCollection<ConditionType>
			{
				ConditionType.IsEqual, ConditionType.IsNotEqual, ConditionType.IsMore, ConditionType.IsNotMore, ConditionType.IsLess, ConditionType.IsNotLess
			};
			var automationChanged = ServiceFactory.SaveService.AutomationChanged;
			SelectedDevicePropertyType = FindObjectCondition.DevicePropertyType;
			SelectedConditionType = FindObjectCondition.ConditionType;
			IntValue = FindObjectCondition.IntValue;
			StringValue = FindObjectCondition.StringValue;
			ServiceFactory.SaveService.AutomationChanged = automationChanged;
		}

		public PropertyType PropertyType { get; private set; }
		public ObservableCollection<DevicePropertyType> DevicePropertyTypes { get; private set; }
		public DevicePropertyType SelectedDevicePropertyType
		{
			get { return FindObjectCondition.DevicePropertyType; }
			set
			{
				FindObjectCondition.DevicePropertyType = value;
				switch (value)
				{
					case DevicePropertyType.ShleifNo:
					case DevicePropertyType.IntAddress:
					case DevicePropertyType.DeviceState:
						PropertyType = PropertyType.Integer; break;
					case DevicePropertyType.Name:
						PropertyType = PropertyType.String; break;
				}
				OnPropertyChanged(()=>SelectedDevicePropertyType);
				OnPropertyChanged(() => PropertyType);
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

		public int IntValue
		{
			get { return FindObjectCondition.IntValue; }
			set
			{
				FindObjectCondition.IntValue = value;
				OnPropertyChanged(() => IntValue);
			}
		}

		public string StringValue
		{
			get { return FindObjectCondition.StringValue; }
			set
			{
				FindObjectCondition.StringValue = value;
				OnPropertyChanged(() => StringValue);
			}
		}
	}
}
