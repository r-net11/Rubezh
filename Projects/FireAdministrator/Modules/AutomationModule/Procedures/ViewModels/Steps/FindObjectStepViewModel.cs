using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.GK;
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
				var findObjectConditionViewModel = new FindObjectConditionViewModel(findObjectCondition);
				FindObjectConditions.Add(findObjectConditionViewModel);
			}
			else
				FindObjectArguments.FindObjectConditions = new List<FindObjectCondition>();
			OnPropertyChanged(()=>Variables);
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

		public void FindObjectsOr(VariableViewModel result, ObservableCollection<FindObjectConditionViewModel> findObjectConditionsViewModel)
		{
			var resultObjects = new List<object>();
			IEnumerable<object> items;
			result.Variable.ObjectsUids = new List<Guid>();
			if (SelectedVariable.ObjectType == ObjectType.Device)
				items = new List<XDevice>(XManager.DeviceConfiguration.Devices);
			else
				items = new List<XDevice>();
			foreach (var item in items)
			{
				var device = new XDevice();
				var itemUid = new Guid();
				if (item is XDevice)
				{
					device = item as XDevice;
					itemUid = device.UID;
				}

				foreach (var findObjectConditionViewModel in findObjectConditionsViewModel)
				{
					var findObjectCondition = findObjectConditionViewModel.FindObjectCondition;
					int intPropertyValue = 0;
					string stringPropertyValue = "";
					switch (findObjectCondition.DevicePropertyType)
					{
						case DevicePropertyType.ShleifNo:
							intPropertyValue = device.ShleifNo;
							break;
						case DevicePropertyType.IntAddress:
							intPropertyValue = device.IntAddress;
							break;
						case DevicePropertyType.DeviceState:
							intPropertyValue = (int)device.State.StateClass;
							break;
						case DevicePropertyType.Name:
							stringPropertyValue = device.PresentationName.Trim();
							break;
					}

					if (resultObjects.Contains(item))
						continue;
					if (((findObjectConditionViewModel.PropertyType == PropertyType.Integer) &&
						(((findObjectCondition.ConditionType == ConditionType.IsEqual) && (intPropertyValue == findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotEqual) && (intPropertyValue != findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsMore) && (intPropertyValue > findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotMore) && (intPropertyValue <= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsLess) && (intPropertyValue < findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotLess) && (intPropertyValue >= findObjectCondition.IntValue)))) ||
						((findObjectConditionViewModel.PropertyType == PropertyType.String) &&
						(((findObjectCondition.StringConditionType == StringConditionType.StartsWith) && (stringPropertyValue.StartsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.EndsWith) && (stringPropertyValue.EndsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.Contains) && (stringPropertyValue.Contains(findObjectCondition.StringValue))))))
							{ resultObjects.Add(item); result.Variable.ObjectsUids.Add(itemUid); }
				}
			}
		}

		public void FindObjectsAnd(VariableViewModel result, ObservableCollection<FindObjectConditionViewModel> findObjectConditionsViewModel)
		{
			var resultObjects = new List<object>();
			var tempObjects = new List<object>();
			IEnumerable<object> items;

			if (SelectedVariable.ObjectType == ObjectType.Device)
			{
				items = new List<XDevice>(XManager.DeviceConfiguration.Devices);
				result.Variable.ObjectsUids = new List<Guid>(XManager.DeviceConfiguration.Devices.Select(x => x.UID));
			}
			else
				items = new List<XDevice>();

			resultObjects = new List<object>(items);
			tempObjects = new List<object>(resultObjects);
			foreach (var findObjectConditionViewModel in findObjectConditionsViewModel)
			{
				var findObjectCondition = findObjectConditionViewModel.FindObjectCondition;
				foreach (var item in resultObjects)
				{
					var device = new XDevice();
					var itemUid = new Guid();
					if (item is XDevice)
					{
						device = item as XDevice;
						itemUid = device.UID;
					}
					int intPropertyValue = 0;
					string stringPropertyValue = "";

					switch (findObjectCondition.DevicePropertyType)
					{
						case DevicePropertyType.ShleifNo:
							intPropertyValue = device.ShleifNo;
							break;
						case DevicePropertyType.IntAddress:
							intPropertyValue = device.IntAddress;
							break;
						case DevicePropertyType.DeviceState:
							intPropertyValue = (int)device.State.StateClass;
							break;
						case DevicePropertyType.Name:
							stringPropertyValue = device.PresentationName.Trim();
							break;
					}
					
					if (((findObjectConditionViewModel.PropertyType == PropertyType.Integer) &&
						(((findObjectCondition.ConditionType == ConditionType.IsEqual) && (intPropertyValue != findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotEqual) && (intPropertyValue == findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsMore) && (intPropertyValue <= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotMore) && (intPropertyValue > findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsLess) && (intPropertyValue >= findObjectCondition.IntValue)) ||
						((findObjectCondition.ConditionType == ConditionType.IsNotLess) && (intPropertyValue < findObjectCondition.IntValue)))) ||
						((findObjectConditionViewModel.PropertyType == PropertyType.String) &&
						(((findObjectCondition.StringConditionType == StringConditionType.StartsWith) && (!stringPropertyValue.StartsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.EndsWith) && (!stringPropertyValue.EndsWith(findObjectCondition.StringValue))) ||
						((findObjectCondition.StringConditionType == StringConditionType.Contains) && (!stringPropertyValue.Contains(findObjectCondition.StringValue))))))
							{ tempObjects.Remove(item); result.Variable.ObjectsUids.Remove(itemUid); }
				}
				resultObjects = new List<object>(tempObjects);
			}
		}

		public RelayCommand RunCommand { get; private set; }
		private void OnRun()
		{
			if (JoinOperator == JoinOperator.Or)
				FindObjectsOr(SelectedVariable, FindObjectConditions);
			else
				FindObjectsAnd(SelectedVariable, FindObjectConditions);
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
			ZonePropertyTypes = new ObservableCollection<ZonePropertyType>
			{
				ZonePropertyType.No, ZonePropertyType.ZoneType, ZonePropertyType.Name
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
				OnPropertyChanged(() => SelectedDevicePropertyType);
				OnPropertyChanged(() => PropertyType);
			}
		}

		public ObservableCollection<ZonePropertyType> ZonePropertyTypes { get; private set; }
		public ZonePropertyType SelectedZonePropertyType
		{
			get { return FindObjectCondition.ZonePropertyType; }
			set
			{
				FindObjectCondition.ZonePropertyType = value;
				switch (value)
				{
					case ZonePropertyType.No:
					case ZonePropertyType.ZoneType:
						PropertyType = PropertyType.Integer; break;
					case ZonePropertyType.Name:
						PropertyType = PropertyType.String; break;
				}
				OnPropertyChanged(() => SelectedZonePropertyType);
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

		public ObservableCollection<StringConditionType> StringConditionTypes { get; private set; }
		public StringConditionType SelectedStringConditionType
		{
			get { return FindObjectCondition.StringConditionType; }
			set
			{
				FindObjectCondition.StringConditionType = value;
				OnPropertyChanged(() => SelectedStringConditionType);
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
