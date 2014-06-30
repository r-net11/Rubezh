using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.GK;
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

		public void InitializeProperties(ref int intPropertyValue, ref string stringPropertyValue, ref Guid itemUid, FindObjectCondition findObjectCondition, object item)
		{
			if (item is XDevice)
			{
				switch (findObjectCondition.DevicePropertyType)
				{
					case DevicePropertyType.ShleifNo:
						intPropertyValue = (item as XDevice).ShleifNo;
						break;
					case DevicePropertyType.IntAddress:
						intPropertyValue = (item as XDevice).IntAddress;
						break;
					case DevicePropertyType.DeviceState:
						intPropertyValue = (int) (item as XDevice).State.StateClass;
						break;
					case DevicePropertyType.Name:
						stringPropertyValue = (item as XDevice).PresentationName.Trim();
						break;
				}
				itemUid = (item as XDevice).UID;
			}

			if (item is XZone)
			{
				switch (findObjectCondition.ZonePropertyType)
				{
					case ZonePropertyType.No:
						intPropertyValue = (item as XZone).No;
						break;
					case ZonePropertyType.ZoneType:
						intPropertyValue = (int) (item as XZone).ObjectType;
						break;
					case ZonePropertyType.Name:
						stringPropertyValue = (item as XZone).Name.Trim();
						break;
				}
				itemUid = (item as XZone).UID;
			}
		}

		void InitializeItems(ref IEnumerable<object> items, ref VariableViewModel result)
		{
			result.Variable.ObjectsUids = new List<Guid>();
			if (SelectedVariable.ObjectType == ObjectType.Device)
			{
				items = new List<XDevice>(XManager.DeviceConfiguration.Devices);
				result.Variable.ObjectsUids = new List<Guid>(XManager.DeviceConfiguration.Devices.Select(x => x.UID));
			}
			if (SelectedVariable.ObjectType == ObjectType.Zone)
			{
				items = new List<XZone>(XManager.Zones);
				result.Variable.ObjectsUids = new List<Guid>(XManager.Zones.Select(x => x.UID));
			}
		}

		public void FindObjectsOr(VariableViewModel result, ObservableCollection<FindObjectConditionViewModel> findObjectConditionsViewModel)
		{
			IEnumerable<object> items = new List<object>();
			InitializeItems(ref items, ref result);
			var resultObjects = new List<object>();
			int intPropertyValue = 0;
			string stringPropertyValue = "";
			var itemUid = new Guid();

			foreach (var findObjectConditionViewModel in findObjectConditionsViewModel)
			{
				var findObjectCondition = findObjectConditionViewModel.FindObjectCondition;

				foreach (var item in items)
				{
					InitializeProperties(ref intPropertyValue, ref stringPropertyValue, ref itemUid, findObjectCondition, item);
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
			IEnumerable<object> items = new List<object>();
			InitializeItems(ref items, ref result);
			var resultObjects = new List<object>(items);
			var tempObjects = new List<object>(resultObjects);
			int intPropertyValue = 0;
			string stringPropertyValue = "";
			var itemUid = new Guid();

			foreach (var findObjectConditionViewModel in findObjectConditionsViewModel)
			{
				var findObjectCondition = findObjectConditionViewModel.FindObjectCondition;
				foreach (var item in resultObjects)
				{
					InitializeProperties(ref intPropertyValue, ref stringPropertyValue, ref itemUid, findObjectCondition, item);
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
		Variable ResultVariable { get; set; }

		public FindObjectConditionViewModel(FindObjectCondition findObjectCondition, Variable resultVariable)
		{
			FindObjectCondition = findObjectCondition;
			ResultVariable = resultVariable;
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

		public ObservableCollection<DevicePropertyType> DevicePropertyTypes { get; private set; }
		public DevicePropertyType SelectedDevicePropertyType
		{
			get { return FindObjectCondition.DevicePropertyType; }
			set
			{
				FindObjectCondition.DevicePropertyType = value;
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
							return PropertyType.Integer;
						case DevicePropertyType.Name:
							return PropertyType.String;
					}
				}

				if (ResultVariable.ObjectType == ObjectType.Zone)
				{
					switch (SelectedZonePropertyType)
					{
						case ZonePropertyType.No:
						case ZonePropertyType.ZoneType:
							return PropertyType.Integer;
						case ZonePropertyType.Name:
							return PropertyType.String;
					}
				}
				return PropertyType.String;
			}
		}
	}
}
