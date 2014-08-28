using System.Linq;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Automation;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using FiresecClient;
using FiresecAPI.SKD;
using System;
using ValueType = FiresecAPI.Automation.ValueType;
using System.Collections.Generic;

namespace AutomationModule.ViewModels
{
	public class VariableDetailsViewModel : SaveCancelDialogViewModel
	{
		public Variable Variable { get; private set; }
		public bool IsEditMode { get; private set; }

		public VariableDetailsViewModel(Variable variable, string defaultName, string title)
		{
			ValueTypes = ProcedureHelper.GetEnumObs<ValueType>();
			ObjectTypes = ProcedureHelper.GetEnumObs<ObjectType>();
			VariableItems = new List<VariableItemViewModel>();
			Variable = new Variable();
			if (variable != null)
				Copy(variable);			
			Name = defaultName;
			Title = title;
			Initialize(variable);
		}

		void Copy(Variable variable)
		{
			Name = variable.Name;
			IsList = variable.IsList;
			Variable.IsGlobal = variable.IsGlobal;
			ValueTypes = new ObservableCollection<ValueType> { variable.ValueType };
			SelectedValueType = variable.ValueType;
			SelectedObjectType = variable.ObjectType;
			IsEditMode = true;
			SelectedVariableItem = new VariableItemViewModel(new VariableItem { ObjectUid = variable.ObjectUid });
			foreach (var variableItem in variable.VariableItems)
				VariableItems.Add(new VariableItemViewModel(variableItem));
		}

		void Initialize(Variable variable)
		{
			SelectCommand = new RelayCommand(OnSelect);
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<VariableItemViewModel>(OnRemove);
			ChangeItemCommand = new RelayCommand<VariableItemViewModel>(OnChangeItem);
		}

		public ObservableCollection<ValueType> ValueTypes { get; private set; }
		ValueType _selectedValueType;
		public ValueType SelectedValueType
		{
			get { return _selectedValueType; }
			set
			{
				_selectedValueType = value;
				OnPropertyChanged(() => SelectedValueType);
			}
		}

		public ObservableCollection<ObjectType> ObjectTypes { get; private set; }
		ObjectType _selectedObjectType;
		public ObjectType SelectedObjectType
		{
			get { return _selectedObjectType; }
			set
			{
				_selectedObjectType = value;
				OnPropertyChanged(() => SelectedObjectType);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		bool _isList;
		public bool IsList
		{
			get { return _isList; }
			set
			{
				_isList = value;
				OnPropertyChanged(() => IsList);
			}
		}

		public RelayCommand SelectCommand { get; private set; }
		void OnSelect()
		{
			SelectedVariableItem = SelectItem(SelectedVariableItem);
		}

		public RelayCommand<VariableItemViewModel> ChangeItemCommand { get; private set; }
		public void OnChangeItem(VariableItemViewModel variableItemViewModel)
		{
			variableItemViewModel.Initialize(SelectItem(variableItemViewModel).VariableItem.ObjectUid);
			OnPropertyChanged(() => VariableObjects);
			OnPropertyChanged(() => VariableBools);
		}

		public List<VariableItemViewModel> VariableItems { get; private set; }
		public ObservableCollection<VariableItemViewModel> VariableObjects
		{
			get { return new ObservableCollection<VariableItemViewModel>(VariableItems.FindAll(x => x.VariableItem.ValueType == ValueType.Object)); } 
		}
		public ObservableCollection<VariableItemViewModel> VariableBools
		{
			get { return new ObservableCollection<VariableItemViewModel>(VariableItems.FindAll(x => x.VariableItem.ValueType == ValueType.Boolean)); }
		}
		VariableItemViewModel _selectedVariableItem;
		public VariableItemViewModel SelectedVariableItem
		{
			get { return _selectedVariableItem; }
			set
			{
				_selectedVariableItem = value;
				if (value != null)
					Variable.ObjectUid = _selectedVariableItem.VariableItem.ObjectUid;
				OnPropertyChanged(() => SelectedVariableItem);
			}
		}

		bool _defaultBoolValue;
		public bool DefaultBoolValue
		{
			get { return _defaultBoolValue; }
			set
			{
				_defaultBoolValue = value;
				OnPropertyChanged(() => DefaultBoolValue);
			}
		}

		DateTime _defaultDateTimeValue;
		public DateTime DefaultDateTimeValue
		{
			get { return _defaultDateTimeValue; }
			set
			{
				_defaultDateTimeValue = value;
				OnPropertyChanged(() => DefaultDateTimeValue);
			}
		}

		int _defaultIntValue;
		public int DefaultIntValue
		{
			get { return _defaultIntValue; }
			set
			{
				_defaultIntValue = value;
				OnPropertyChanged(() => DefaultIntValue);
			}
		}

		string _defaultStringValue;
		public string DefaultStringValue
		{
			get { return _defaultStringValue; }
			set
			{
				_defaultStringValue = value;
				OnPropertyChanged(() => DefaultStringValue);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var variableItemViewModel = SelectItem();
			if (!variableItemViewModel.IsEmpty)
				VariableItems.Add(variableItemViewModel);
			OnPropertyChanged(() => VariableObjects);
			OnPropertyChanged(() => VariableBools);
		}

		public RelayCommand<VariableItemViewModel> RemoveCommand { get; private set; }
		void OnRemove(VariableItemViewModel variableItemViewModel)
		{
			if (variableItemViewModel == null)
				return;
			VariableItems.Remove(variableItemViewModel);
			OnPropertyChanged(() => VariableObjects);
			OnPropertyChanged(() => VariableBools);
		}

		VariableItemViewModel SelectItem(VariableItemViewModel currentVariableItem = null)
		{
			if (currentVariableItem == null)
				currentVariableItem = new VariableItemViewModel(new VariableItem());
			var variableItem = new VariableItem();
			variableItem.ValueType = SelectedValueType;
			variableItem.ObjectUid = currentVariableItem.VariableItem.ObjectUid;
			if (SelectedObjectType == ObjectType.Device)
			{
				var deviceSelectationViewModel = new DeviceSelectionViewModel(currentVariableItem.Device != null ? currentVariableItem.Device : null);
				if (DialogService.ShowModalWindow(deviceSelectationViewModel))
					variableItem.ObjectUid = deviceSelectationViewModel.SelectedDevice.Device.UID;
			}

			if (SelectedObjectType == ObjectType.Zone)
			{
				var zoneSelectationViewModel = new ZoneSelectionViewModel(currentVariableItem.Zone != null ? currentVariableItem.Zone : null);
				if (DialogService.ShowModalWindow(zoneSelectationViewModel))
					variableItem.ObjectUid = zoneSelectationViewModel.SelectedZone.Zone.UID;
			}

			if (SelectedObjectType == ObjectType.GuardZone)
			{
				var guardZoneSelectationViewModel = new GuardZoneSelectionViewModel(currentVariableItem.GuardZone != null ? currentVariableItem.GuardZone : null);
				if (DialogService.ShowModalWindow(guardZoneSelectationViewModel))
					variableItem.ObjectUid = guardZoneSelectationViewModel.SelectedZone.GuardZone.UID;
			}

			if (SelectedObjectType == ObjectType.SKDDevice)
			{
				var skdDeviceSelectationViewModel = new SKDDeviceSelectionViewModel(currentVariableItem.SKDDevice != null ? currentVariableItem.SKDDevice : null);
				if (DialogService.ShowModalWindow(skdDeviceSelectationViewModel))
					variableItem.ObjectUid = skdDeviceSelectationViewModel.SelectedDevice.SKDDevice.UID;
			}

			if (SelectedObjectType == ObjectType.SKDZone)
			{
				var skdZoneSelectationViewModel = new SKDZoneSelectionViewModel(currentVariableItem.SKDZone != null ? currentVariableItem.SKDZone : null);
				if (DialogService.ShowModalWindow(skdZoneSelectationViewModel))
					variableItem.ObjectUid = skdZoneSelectationViewModel.SelectedZone.SKDZone.UID;
			}

			if (SelectedObjectType == ObjectType.ControlDoor)
			{
				var doorSelectationViewModel = new DoorSelectionViewModel(currentVariableItem.SKDDoor != null ? currentVariableItem.SKDDoor : null);
				if (DialogService.ShowModalWindow(doorSelectationViewModel))
					variableItem.ObjectUid = doorSelectationViewModel.SelectedDoor.Door.UID;
			}

			if (SelectedObjectType == ObjectType.Direction)
			{
				var directionSelectationViewModel = new DirectionSelectionViewModel(currentVariableItem.Direction != null ? currentVariableItem.Direction : null);
				if (DialogService.ShowModalWindow(directionSelectationViewModel))
					variableItem.ObjectUid = directionSelectationViewModel.SelectedDirection.Direction.UID;
			}

			if (SelectedObjectType == ObjectType.VideoDevice)
			{
				var cameraSelectionViewModel = new CameraSelectionViewModel(currentVariableItem.Camera != null ? currentVariableItem.Camera : null);
				if (DialogService.ShowModalWindow(cameraSelectionViewModel))
					variableItem.ObjectUid = cameraSelectionViewModel.SelectedCamera.Camera.UID;
			}

			return new VariableItemViewModel(variableItem);
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}
			Variable.Name = Name;
			Variable.DefaultBoolValue = DefaultBoolValue;
			Variable.DefaultDateTimeValue = DefaultDateTimeValue;
			Variable.DefaultIntValue = DefaultIntValue;
			Variable.DefaultStringValue = DefaultStringValue;
			Variable.ValueType = SelectedValueType;
			Variable.ObjectType = SelectedObjectType;
			Variable.IsList = IsList;
			Variable.VariableItems = new List<VariableItem>();
			foreach (var variableItemViewModel in VariableItems)
				Variable.VariableItems.Add(variableItemViewModel.VariableItem);
			return base.Save();
		}
	}
}