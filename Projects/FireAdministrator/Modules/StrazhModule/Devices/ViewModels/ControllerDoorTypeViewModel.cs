using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using StrazhModule.Properties;

namespace StrazhModule.ViewModels
{
	public class ControllerDoorTypeViewModel : SaveCancelDialogViewModel
	{
		DeviceViewModel DeviceViewModel { get; set; }

		public ControllerDoorTypeViewModel(DeviceViewModel deviceViewModel)
		{
			Title = "Основные настройки контроллера";
			DeviceViewModel = deviceViewModel;

			ReadFromControllerCommand = new RelayCommand(OnReadFromController);
			WriteToControllerCommand = new RelayCommand(OnWriteToController);

			AvailableDoorTypes = new ObservableCollection<DoorType> {DoorType.OneWay, DoorType.TwoWay};
			SelectedDoorType = DeviceViewModel.Device.DoorType;

			if (DeviceViewModel.Device.AntiPassBackConfiguration != null)
			{
				UpdateAntiPassBackModesAvailability();
				IsAntiPassBackActivated = DeviceViewModel.Device.AntiPassBackConfiguration.IsActivated;
				SelectedAntiPassBackMode = DeviceViewModel.Device.AntiPassBackConfiguration.CurrentAntiPassBackMode;
			}

			if (DeviceViewModel.Device.InterlockConfiguration != null)
			{
				UpdateInterlockModesAvailability();
				IsInterlockActivated = DeviceViewModel.Device.InterlockConfiguration.IsActivated;
				SelectedInterlockMode = DeviceViewModel.Device.InterlockConfiguration.CurrentInterlockMode;
			}

			HasChanged = false;
			NeedSaveChangesToController = false;
		}

		#region <Флаги изменения настроек контроллера>

		bool SelectedDoorTypeHasChanged { get; set; }
		bool AntiPassBackHasChanged { get; set; }
		bool InterlockHasChanged { get; set; }
		bool HasChanged
		{
			get
			{
				return SelectedDoorTypeHasChanged || AntiPassBackHasChanged || InterlockHasChanged;
			}
			set
			{
				SelectedDoorTypeHasChanged = value;
				AntiPassBackHasChanged = value;
				InterlockHasChanged = value;
			}
		}

		bool NeedSaveChangesToController { get; set; }

		#endregion </Флаги изменения настроек контроллера>
		
		#region <Тип точки доступа>

		public ObservableCollection<DoorType> AvailableDoorTypes { get; private set; }

		DoorType _selectedDoorType;
		public DoorType SelectedDoorType
		{
			get { return _selectedDoorType; }
			set
			{
				if (_selectedDoorType.Equals(value))
					return;
				_selectedDoorType = value;
				OnPropertyChanged(() => SelectedDoorType);
				
				IsAntiPassBackActivated = false;
				IsInterlockActivated = false;

				UpdateAntiPassBackModesAvailability();
				UpdateInterlockModesAvailability();
				
				SelectedDoorTypeHasChanged = true;
				NeedSaveChangesToController = true;
			}
		}

		#endregion </Тип точки доступа>

		#region <Запрет повторного прохода>

		bool _isAntiPassBackEnabled;
		public bool IsAntiPassBackEnabled
		{
			get { return _isAntiPassBackEnabled; }
			set
			{
				if (_isAntiPassBackEnabled.Equals(value))
					return;
				_isAntiPassBackEnabled = value;
				OnPropertyChanged(() => IsAntiPassBackEnabled);
			}
		}

		bool _isAntiPassBackModeR1InR2OutEnabled;
		public bool IsAntiPassBackModeR1InR2OutEnabled
		{
			get { return IsAntiPassBackActivated && _isAntiPassBackModeR1InR2OutEnabled; }
			set
			{
				_isAntiPassBackModeR1InR2OutEnabled = value;
				OnPropertyChanged(() => IsAntiPassBackModeR1InR2OutEnabled);
			}
		}

		bool _isAntiPassBackModeR3InR4OutEnabled;
		public bool IsAntiPassBackModeR3InR4OutEnabled
		{
			get { return IsAntiPassBackActivated && _isAntiPassBackModeR3InR4OutEnabled; }
			set
			{
				_isAntiPassBackModeR3InR4OutEnabled = value;
				OnPropertyChanged(() => IsAntiPassBackModeR3InR4OutEnabled);
			}
		}

		bool _isAntiPassBackModeR1R3InR2R4OutEnabled;
		public bool IsAntiPassBackModeR1R3InR2R4OutEnabled
		{
			get { return IsAntiPassBackActivated && _isAntiPassBackModeR1R3InR2R4OutEnabled; }
			set
			{
				_isAntiPassBackModeR1R3InR2R4OutEnabled = value;
				OnPropertyChanged(() => IsAntiPassBackModeR1R3InR2R4OutEnabled);
			}
		}

		bool _isAntiPassBackActivated;
		public bool IsAntiPassBackActivated
		{
			get { return _isAntiPassBackActivated && !SelectedDoorType.Equals(DoorType.OneWay); }
			set
			{
				if (_isAntiPassBackActivated.Equals(value))
					return;
				_isAntiPassBackActivated = value;
				OnPropertyChanged(() => IsAntiPassBackActivated);
				UpdateAntiPassBackModesAvailability();
				AntiPassBackHasChanged = true;
				NeedSaveChangesToController = true;
			}
		}

		SKDAntiPassBackMode _selectedAntiPassBackMode;
		public SKDAntiPassBackMode SelectedAntiPassBackMode
		{
			get { return _selectedAntiPassBackMode; }
			set
			{
				if (_selectedAntiPassBackMode.Equals(value)) return;
				_selectedAntiPassBackMode = value;
				OnPropertyChanged(() => SelectedAntiPassBackMode);
				AntiPassBackHasChanged = true;
				NeedSaveChangesToController = true;
			}
		}

		void UpdateAntiPassBackModesAvailability()
		{
			// Для двухсторонней точки доступа выборочно активировать режимы работы Anti-pass Back
			if (SelectedDoorType == DoorType.TwoWay)
			{
				switch (DeviceViewModel.Device.DriverType)
				{
					// Однодверник
					case SKDDriverType.ChinaController_1:
						IsAntiPassBackModeR1InR2OutEnabled = true;
						IsAntiPassBackModeR3InR4OutEnabled = false;
						IsAntiPassBackModeR1R3InR2R4OutEnabled = false;
						break;
					// Двухдверник
					case SKDDriverType.ChinaController_2:
						IsAntiPassBackModeR1InR2OutEnabled = true;
						IsAntiPassBackModeR3InR4OutEnabled = false;
						IsAntiPassBackModeR1R3InR2R4OutEnabled = false;
						break;
					// Четырехдверник
					case SKDDriverType.ChinaController_4:
						IsAntiPassBackModeR1InR2OutEnabled = true;
						IsAntiPassBackModeR3InR4OutEnabled = true;
						IsAntiPassBackModeR1R3InR2R4OutEnabled = true;
						break;
				}
			}
			// Для прочих точек доступа Anti-pass Back не доступен
			else
			{
				IsAntiPassBackModeR1InR2OutEnabled = false;
				IsAntiPassBackModeR3InR4OutEnabled = false;
				IsAntiPassBackModeR1R3InR2R4OutEnabled = false;
			}
			IsAntiPassBackEnabled = _isAntiPassBackModeR1InR2OutEnabled
				|| _isAntiPassBackModeR3InR4OutEnabled
				|| _isAntiPassBackModeR1R3InR2R4OutEnabled;

		}

		#endregion </Запрет повторного прохода>

		#region <Блокировка одновременного прохода>

		bool _isInterlockEnabled;
		public bool IsInterlockEnabled
		{
			get { return _isInterlockEnabled; }
			set
			{
				if (_isInterlockEnabled.Equals(value)) return;
				_isInterlockEnabled = value;
				OnPropertyChanged(() => IsInterlockEnabled);
			}
		}

		bool _isInterlockModeL1L2Enabled;
		public bool IsInterlockModeL1L2Enabled
		{
			get { return IsInterlockActivated && _isInterlockModeL1L2Enabled; }
			set
			{
				_isInterlockModeL1L2Enabled = value;
				OnPropertyChanged(() => IsInterlockModeL1L2Enabled);
			}
		}

		bool _isInterlockModeL1L2L3Enabled;
		public bool IsInterlockModeL1L2L3Enabled
		{
			get { return IsInterlockActivated && _isInterlockModeL1L2L3Enabled; }
			set
			{
				_isInterlockModeL1L2L3Enabled = value;
				OnPropertyChanged(() => IsInterlockModeL1L2L3Enabled);
			}
		}

		bool _isInterlockModeL1L2L3L4Enabled;
		public bool IsInterlockModeL1L2L3L4Enabled
		{
			get { return IsInterlockActivated && _isInterlockModeL1L2L3L4Enabled; }
			set
			{
				_isInterlockModeL1L2L3L4Enabled = value;
				OnPropertyChanged(() => IsInterlockModeL1L2L3L4Enabled);
			}
		}

		bool _isInterlockModeL2L3L4Enabled;
		public bool IsInterlockModeL2L3L4Enabled
		{
			get { return IsInterlockActivated && _isInterlockModeL2L3L4Enabled; }
			set
			{
				_isInterlockModeL2L3L4Enabled = value;
				OnPropertyChanged(() => IsInterlockModeL2L3L4Enabled);
			}
		}

		bool _isInterlockModeL1L3_L2L4Enabled;
		public bool IsInterlockModeL1L3_L2L4Enabled
		{
			get { return IsInterlockActivated && _isInterlockModeL1L3_L2L4Enabled; }
			set
			{
				_isInterlockModeL1L3_L2L4Enabled = value;
				OnPropertyChanged(() => IsInterlockModeL1L3_L2L4Enabled);
			}
		}

		bool _isInterlockModeL1L4_L2L3Enabled;
		public bool IsInterlockModeL1L4_L2L3Enabled
		{
			get { return IsInterlockActivated && _isInterlockModeL1L4_L2L3Enabled; }
			set
			{
				_isInterlockModeL1L4_L2L3Enabled = value;
				OnPropertyChanged(() => IsInterlockModeL1L4_L2L3Enabled);
			}
		}

		bool _isInterlockModeL3L4Enabled;
		public bool IsInterlockModeL3L4Enabled
		{
			get { return IsInterlockActivated && _isInterlockModeL3L4Enabled; }
			set
			{
				_isInterlockModeL3L4Enabled = value;
				OnPropertyChanged(() => IsInterlockModeL3L4Enabled);
			}
		}

		bool _isInterlockActivated;
		public bool IsInterlockActivated
		{
			get { return _isInterlockActivated; }
			set
			{
				if (_isInterlockActivated.Equals(value)) return;
				_isInterlockActivated = value;
				OnPropertyChanged(() => IsInterlockActivated);
				UpdateInterlockModesAvailability();
				InterlockHasChanged = true;
				NeedSaveChangesToController = true;
			}
		}

		SKDInterlockMode _selectedInterlockMode;
		public SKDInterlockMode SelectedInterlockMode
		{
			get { return _selectedInterlockMode; }
			set
			{
				if (_selectedInterlockMode.Equals(value)) return;
				_selectedInterlockMode = value;
				OnPropertyChanged(() => SelectedInterlockMode);
				InterlockHasChanged = true;
				NeedSaveChangesToController = true;
			}
		}

		void UpdateInterlockModesAvailability()
		{
			switch (SelectedDoorType)
			{
				case DoorType.TwoWay:
					switch (DeviceViewModel.Device.DriverType)
					{
						// Однодверник
						case SKDDriverType.ChinaController_1:
						// Двухдверник
						case SKDDriverType.ChinaController_2:
							IsInterlockModeL1L2Enabled = false;
							IsInterlockModeL1L2L3Enabled = false;
							IsInterlockModeL1L2L3L4Enabled = false;
							IsInterlockModeL2L3L4Enabled = false;
							IsInterlockModeL1L3_L2L4Enabled = false;
							IsInterlockModeL1L4_L2L3Enabled = false;
							IsInterlockModeL3L4Enabled = false;
							_isInterlockActivated = false;
							break;
						// Четырехдверник
						case SKDDriverType.ChinaController_4:
							IsInterlockModeL1L2Enabled = true;
							IsInterlockModeL1L2L3Enabled = false;
							IsInterlockModeL1L2L3L4Enabled = false;
							IsInterlockModeL2L3L4Enabled = false;
							IsInterlockModeL1L3_L2L4Enabled = false;
							IsInterlockModeL1L4_L2L3Enabled = false;
							IsInterlockModeL3L4Enabled = false;
							break;
					}
					break;
				case DoorType.OneWay:
					switch (DeviceViewModel.Device.DriverType)
					{
						// Однодверник
						case SKDDriverType.ChinaController_1:
							IsInterlockModeL1L2Enabled = false;
							IsInterlockModeL1L2L3Enabled = false;
							IsInterlockModeL1L2L3L4Enabled = false;
							IsInterlockModeL2L3L4Enabled = false;
							IsInterlockModeL1L3_L2L4Enabled = false;
							IsInterlockModeL1L4_L2L3Enabled = false;
							IsInterlockModeL3L4Enabled = false;
							_isInterlockActivated = false;
							break;
						// Двухдверник
						case SKDDriverType.ChinaController_2:
							IsInterlockModeL1L2Enabled = true;
							IsInterlockModeL1L2L3Enabled = false;
							IsInterlockModeL1L2L3L4Enabled = false;
							IsInterlockModeL2L3L4Enabled = false;
							IsInterlockModeL1L3_L2L4Enabled = false;
							IsInterlockModeL1L4_L2L3Enabled = false;
							IsInterlockModeL3L4Enabled = false;
							break;
						// Четырехдверник
						case SKDDriverType.ChinaController_4:
							IsInterlockModeL1L2Enabled = true;
							IsInterlockModeL1L2L3Enabled = true;
							IsInterlockModeL1L2L3L4Enabled = true;
							IsInterlockModeL2L3L4Enabled = true;
							IsInterlockModeL1L3_L2L4Enabled = true;
							IsInterlockModeL1L4_L2L3Enabled = true;
							IsInterlockModeL3L4Enabled = true;
							break;
					}
					break;
			}
			IsInterlockEnabled = _isInterlockModeL1L2Enabled
				|| _isInterlockModeL1L2L3Enabled
				|| _isInterlockModeL1L2L3L4Enabled
				|| _isInterlockModeL2L3L4Enabled
				|| _isInterlockModeL1L3_L2L4Enabled
				|| _isInterlockModeL1L4_L2L3Enabled
				|| _isInterlockModeL3L4Enabled;

		}

		#endregion </Блокировка одновременного прохода>

		bool GetDoorType()
		{
			var result = FiresecManager.FiresecService.GetControllerDoorType(DeviceViewModel.Device);
			if (result.HasError)
				return false;
			SelectedDoorType = result.Result;
			return true;
		}
		bool SetDoorType()
		{
			if (!SelectedDoorTypeHasChanged)
				return true;

			var result = FiresecManager.FiresecService.SetControllerDoorType(DeviceViewModel.Device, SelectedDoorType);
			return !result.HasError;
		}

		bool GetAntiPassBack()
		{
			var result = FiresecManager.FiresecService.SKDGetAntiPassBackConfiguration(DeviceViewModel.Device);
			if (result.HasError)
				return false;
			IsAntiPassBackActivated = result.Result.IsActivated;
			SelectedAntiPassBackMode = result.Result.CurrentAntiPassBackMode;
			return true;
		}
		bool SetAntiPassBack()
		{
			if (!AntiPassBackHasChanged)
				return true;

			var result = FiresecManager.FiresecService.SKDSetAntiPassBackConfiguration(
				DeviceViewModel.Device,
				new SKDAntiPassBackConfiguration() { IsActivated = IsAntiPassBackActivated , CurrentAntiPassBackMode = SelectedAntiPassBackMode });
			return !result.HasError;
		}

		bool GetInterlock()
		{
			var result = FiresecManager.FiresecService.SKDGetInterlockConfiguration(DeviceViewModel.Device);
			if (result.HasError)
				return false;
			IsInterlockActivated = result.Result.IsActivated;
			SelectedInterlockMode = result.Result.CurrentInterlockMode;
			return true;
		}
		bool SetInterlock()
		{
			if (!InterlockHasChanged)
				return true;

			var result = FiresecManager.FiresecService.SKDSetInterlockConfiguration(
				DeviceViewModel.Device,
				new SKDInterlockConfiguration() { IsActivated = IsInterlockActivated, CurrentInterlockMode = SelectedInterlockMode });
			return !result.HasError;
		}

		public RelayCommand ReadFromControllerCommand { get; private set; }
		void OnReadFromController()
		{
			var status = GetDoorType();
			status &= GetAntiPassBack();
			status &= GetInterlock();
			if (!status)
			{
				MessageBoxService.ShowWarning("Нет связи с устройством");
				return;
			}
			NeedSaveChangesToController = false;
		}
		
		public RelayCommand WriteToControllerCommand { get; private set; }
		void OnWriteToController()
		{
			var status = SetAntiPassBack();
			status &= SetInterlock();
			status &= SetDoorType(); // Выполняется последней, т.к. при этой операции происходит перезагрузка контроллера
			
			if (!status)
			{
				MessageBoxService.ShowWarning("Нет связи с устройством");
				return;				
			}
			if (SelectedDoorTypeHasChanged)
				MessageBoxService.Show("Выполняется перезагрузка контроллера. Контроллер будет доступен через несколько секунд");

			NeedSaveChangesToController = false;
		}

		protected override bool Save()
		{
			if (HasChanged)
			{
				DeviceViewModel.Device.DoorType = SelectedDoorType;
				DeviceViewModel.Device.AntiPassBackConfiguration = new SKDAntiPassBackConfiguration { IsActivated = IsAntiPassBackActivated, CurrentAntiPassBackMode = SelectedAntiPassBackMode };
				DeviceViewModel.Device.InterlockConfiguration = new SKDInterlockConfiguration { IsActivated = IsInterlockActivated, CurrentInterlockMode = SelectedInterlockMode };
				ServiceFactory.SaveService.SKDChanged = true;
			}
			if (NeedSaveChangesToController && !MessageBoxService.ShowConfirmation(Resources.SaveConfigurationControllerWarning))
			{
				return false;
			}
			return base.Save();
		}
	}
}