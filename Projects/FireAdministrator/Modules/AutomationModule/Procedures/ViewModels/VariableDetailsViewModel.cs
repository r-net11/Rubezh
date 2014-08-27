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

		public VariableDetailsViewModel(bool isArgument = false, bool isGlobal = false)
		{
			var defaultName = "Локальная переменная";
			var title = "Добавить переменную";
			if (isArgument)
			{
				defaultName = "Аргумент";
				title = "Добавить аргумент";
			}
			if (isGlobal)
			{
				defaultName = "Глобальная переменная";
				title = "Добавить глобальную переменную";
			}
			Title = title;
			Variable = new Variable(defaultName);
			Variable.IsGlobal = isGlobal;
			Variables = new ObservableCollection<VariableViewModel>
			{
				new VariableViewModel(defaultName, ValueType.Integer),
				new VariableViewModel(defaultName, ValueType.Boolean),
				new VariableViewModel(defaultName, ValueType.String),
				new VariableViewModel(defaultName, ValueType.DateTime),
				new VariableViewModel(defaultName, ValueType.Object)
			};
			SelectedVariable = Variables.FirstOrDefault();
			Name = defaultName;
			IsEditMode = false;
			SelectCommand = new RelayCommand(OnSelect);
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<VariableObjectViewModel>(OnRemove);
			ChangeItemCommand = new RelayCommand<VariableObjectViewModel>(OnChangeItem);
			SelectedVariableObject = new VariableObjectViewModel(Variable.ObjectUid);
			InitializeVariableObjects();
		}

		public VariableDetailsViewModel(Variable variable, bool isArgument = false)
		{
			var title = "Редактировать переменную";
			if (isArgument)
			{
				title = "Редактировать аргумент";
			}
			Title = title;
			Variable = new Variable(variable);
			Variables = new ObservableCollection<VariableViewModel>
			{
				 new VariableViewModel(variable)
			};
			SelectedVariable = Variables.FirstOrDefault(x => x.ValueType == variable.ValueType);
			if (SelectedVariable != null)
			{
				Name = SelectedVariable.Name;
				IsList = SelectedVariable.IsList;
			}
			IsEditMode = true;
			SelectCommand = new RelayCommand(OnSelect);
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand<VariableObjectViewModel>(OnRemove);
			ChangeItemCommand = new RelayCommand<VariableObjectViewModel>(OnChangeItem);
			SelectedVariableObject = new VariableObjectViewModel(Variable.ObjectUid);
			InitializeVariableObjects();
		}

		void InitializeVariableObjects()
		{
			VariableObjects = new ObservableCollection<VariableObjectViewModel>();
			foreach (var objectUid in SelectedVariable.Variable.ObjectsUids)
				VariableObjects.Add(new VariableObjectViewModel(objectUid));
		}

		VariableViewModel _selectedVariable;
		public VariableViewModel SelectedVariable
		{
			get { return _selectedVariable; }
			set
			{
				_selectedVariable = value;
				OnPropertyChanged(() => SelectedVariable);
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
		private void OnSelect()
		{			
			if (SelectedVariable.ObjectType == ObjectType.Device)
			{
				var deviceSelectationViewModel = new DeviceSelectionViewModel(SelectedVariableObject.Device != null ? SelectedVariableObject.Device : null);
				if (DialogService.ShowModalWindow(deviceSelectationViewModel))
					SelectedVariableObject = new VariableObjectViewModel(deviceSelectationViewModel.SelectedDevice.Device.UID);
			}

			if (SelectedVariable.ObjectType == ObjectType.Zone)
			{
				var zoneSelectationViewModel = new ZoneSelectionViewModel(SelectedVariableObject.Zone != null ? SelectedVariableObject.Zone : null);
				if (DialogService.ShowModalWindow(zoneSelectationViewModel))
					SelectedVariableObject = new VariableObjectViewModel(zoneSelectationViewModel.SelectedZone.Zone.UID);
			}

			if (SelectedVariable.ObjectType == ObjectType.GuardZone)
			{
				var guardZoneSelectationViewModel = new GuardZoneSelectionViewModel(SelectedVariableObject.GuardZone != null ? SelectedVariableObject.GuardZone : null);
				if (DialogService.ShowModalWindow(guardZoneSelectationViewModel))
					SelectedVariableObject = new VariableObjectViewModel(guardZoneSelectationViewModel.SelectedZone.GuardZone.UID);
			}

			if (SelectedVariable.ObjectType == ObjectType.SKDDevice)
			{
				var skdDeviceSelectationViewModel = new SKDDeviceSelectionViewModel(SelectedVariableObject.SKDDevice != null ? SelectedVariableObject.SKDDevice : null);
				if (DialogService.ShowModalWindow(skdDeviceSelectationViewModel))
					SelectedVariableObject = new VariableObjectViewModel(skdDeviceSelectationViewModel.SelectedDevice.SKDDevice.UID);
			}

			if (SelectedVariable.ObjectType == ObjectType.SKDZone)
			{
				var skdZoneSelectationViewModel = new SKDZoneSelectionViewModel(SelectedVariableObject.SKDZone != null ? SelectedVariableObject.SKDZone : null);
				if (DialogService.ShowModalWindow(skdZoneSelectationViewModel))
					SelectedVariableObject = new VariableObjectViewModel(skdZoneSelectationViewModel.SelectedZone.SKDZone.UID);
			}

			if (SelectedVariable.ObjectType == ObjectType.ControlDoor)
			{
				var doorSelectationViewModel = new DoorSelectionViewModel(SelectedVariableObject.SKDDoor != null ? SelectedVariableObject.SKDDoor : null);
				if (DialogService.ShowModalWindow(doorSelectationViewModel))
					SelectedVariableObject = new VariableObjectViewModel(doorSelectationViewModel.SelectedDoor.Door.UID);
			}

			if (SelectedVariable.ObjectType == ObjectType.Direction)
			{
				var directionSelectationViewModel = new DirectionSelectionViewModel(SelectedVariableObject.Direction != null ? SelectedVariableObject.Direction : null);
				if (DialogService.ShowModalWindow(directionSelectationViewModel))
					SelectedVariableObject = new VariableObjectViewModel(directionSelectationViewModel.SelectedDirection.Direction.UID);
			}

			if (SelectedVariable.ObjectType == ObjectType.VideoDevice)
			{
				var cameraSelectionViewModel = new CameraSelectionViewModel(SelectedVariableObject.Camera != null ? SelectedVariableObject.Camera : null);
				if (DialogService.ShowModalWindow(cameraSelectionViewModel))
					SelectedVariableObject = new VariableObjectViewModel(cameraSelectionViewModel.SelectedCamera.Camera.UID);
			}
		}

		public RelayCommand<VariableObjectViewModel> ChangeItemCommand { get; private set; }
		public void OnChangeItem(VariableObjectViewModel variableObjectViewModel)
		{
			SelectedVariableObject = variableObjectViewModel;
			OnSelect();
			variableObjectViewModel.Copy(SelectedVariableObject);
			OnPropertyChanged(() => VariableObjects);
		}

		public ObservableCollection<VariableObjectViewModel> VariableObjects { get; private set; }
		VariableObjectViewModel _selectedVariableObject;
		public VariableObjectViewModel SelectedVariableObject
		{
			get { return _selectedVariableObject; }
			set
			{
				_selectedVariableObject = value;
				if (value != null)
					SelectedVariable.Variable.ObjectUid = _selectedVariableObject.ObjectUid;
				OnPropertyChanged(() => SelectedVariableObject);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			SelectedVariableObject.IsEmpty = true;
			OnSelect();
			if (!SelectedVariableObject.IsEmpty)
				VariableObjects.Add(SelectedVariableObject);
			OnPropertyChanged(() => VariableObjects);
		}

		public RelayCommand<VariableObjectViewModel> RemoveCommand { get; private set; }
		void OnRemove(VariableObjectViewModel variableObjectViewModel)
		{
			if (variableObjectViewModel != null)
				VariableObjects.Remove(variableObjectViewModel);
			OnPropertyChanged(() => VariableObjects);
		}

		protected override bool Save()
		{
			if (string.IsNullOrEmpty(Name))
			{
				MessageBoxService.ShowWarning("Название не может быть пустым");
				return false;
			}
			Variable.Name = SelectedVariable.Name;
			Variable.DefaultBoolValue = SelectedVariable.DefaultBoolValue;
			Variable.DefaultDateTimeValue = SelectedVariable.DefaultDateTimeValue;
			Variable.DefaultIntValue = SelectedVariable.DefaultIntValue;
			Variable.Name = Name;
			Variable.ObjectType = SelectedVariable.ObjectType;
			Variable.DefaultStringValue = SelectedVariable.DefaultStringValue;
			Variable.ValueType = SelectedVariable.ValueType;
			Variable.IsList = IsList;
			Variable.ObjectUid = SelectedVariable.Variable.ObjectUid;
			Variable.ObjectsUids = new List<Guid>();
			foreach (var variableObject in VariableObjects)
				Variable.ObjectsUids.Add(variableObject.ObjectUid);
			return base.Save();
		}
	}
}