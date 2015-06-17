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
			Title = "Задание типа контроллера";
			DeviceViewModel = deviceViewModel;

			ReadFromControllerCommand = new RelayCommand(OnReadFromController);
			WriteToControllerCommand = new RelayCommand(OnWriteToController);

			AvailableDoorTypes = new ObservableCollection<DoorType> {DoorType.OneWay, DoorType.TwoWay};
			SelectedDoorType = DeviceViewModel.Device.DoorType;

			if (DeviceViewModel.Device.AntiPassBackConfiguration != null)
			{
				UpdateAntiPassBackModesAvailability();
				SelectedAntiPassBackMode = DeviceViewModel.Device.AntiPassBackConfiguration.CurrentAntiPassBackMode;
			}

			if (DeviceViewModel.Device.InterlockConfiguration != null)
			{
				UpdateInterlockModesAvailability();
				SelectedInterlockMode = DeviceViewModel.Device.InterlockConfiguration.CurrentInterlockMode;
			}

			HasChanged = false;
		}

		#region Флаги изменения настроек контроллера

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

		#endregion // Флаги изменения настроек контроллера
		
		#region Тип точки доступа

		public ObservableCollection<DoorType> AvailableDoorTypes { get; private set; }

		DoorType _selectedDoorType;
		public DoorType SelectedDoorType
		{
			get { return _selectedDoorType; }
			set
			{
				_selectedDoorType = value;
				OnPropertyChanged(() => SelectedDoorType);
				IsAntiPassBackActivated = false;
				IsInterlockActivated = false;
				SelectedDoorTypeHasChanged = true;
			}
		}

		#endregion // Тип точки доступа

		#region Запрет повторного прохода
		
		bool _isAntiPassBackActivated;
		public bool IsAntiPassBackActivated
		{
			get { return _isAntiPassBackActivated && !SelectedDoorType.Equals(DoorType.OneWay); }
			set
			{
				if (!_isAntiPassBackActivated.Equals(value))
				{
					_isAntiPassBackActivated = value;
					OnPropertyChanged(() => IsAntiPassBackActivated);
					UpdateAntiPassBackModesAvailability();
					AntiPassBackHasChanged = true;
				}
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

		SKDAntiPassBackMode _selectedAntiPassBackMode;
		public SKDAntiPassBackMode SelectedAntiPassBackMode
		{
			get { return _selectedAntiPassBackMode; }
			set
			{
				if (!_selectedAntiPassBackMode.Equals(value))
				{
					_selectedAntiPassBackMode = value;
					OnPropertyChanged(() => SelectedAntiPassBackMode);
					AntiPassBackHasChanged = true;
				}
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
		}

		#endregion // Запрет повторного прохода

		#region Блокировка прохода

		bool _isInterlockActivated;
		public bool IsInterlockActivated
		{
			get { return _isInterlockActivated; }
			set
			{
				if (!_isInterlockActivated.Equals(value))
				{
					_isInterlockActivated = value;
					OnPropertyChanged(() => IsInterlockActivated);
					UpdateInterlockModesAvailability();
					InterlockHasChanged = true;
				}
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

		SKDInterlockMode _selectedInterlockMode;
		public SKDInterlockMode SelectedInterlockMode
		{
			get { return _selectedInterlockMode; }
			set
			{
				if (!_selectedInterlockMode.Equals(value))
				{
					_selectedInterlockMode = value;
					OnPropertyChanged(() => SelectedInterlockMode);
					InterlockHasChanged = true;
				}
			}
		}

		void UpdateInterlockModesAvailability()
		{
			switch (SelectedDoorType)
			{
				case DoorType.OneWay:
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
				case DoorType.TwoWay:
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
							IsInterlockModeL2L3L4Enabled = false;
							IsInterlockModeL1L3_L2L4Enabled = false;
							IsInterlockModeL1L4_L2L3Enabled = false;
							IsInterlockModeL3L4Enabled = false;
							break;
					}
					break;
			}
		}

		#endregion // Блокировка прохода

		void GetDoorType()
		{
			var result = FiresecManager.FiresecService.GetControllerDoorType(DeviceViewModel.Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				if (SelectedDoorType != result.Result)
				{
					SelectedDoorType = result.Result;
					SelectedDoorTypeHasChanged = true;
				}
			}
		}
		bool SetDoorType()
		{
			if (!SelectedDoorTypeHasChanged)
				return false;

			var result = FiresecManager.FiresecService.SetControllerDoorType(DeviceViewModel.Device, SelectedDoorType);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return false;
			}

			SelectedDoorTypeHasChanged = false;
			return true;
		}

		void GetAntiPassBack()
		{
			var result = FiresecManager.FiresecService.SKDGetAntiPassBackConfiguration(DeviceViewModel.Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				if (IsAntiPassBackActivated != result.Result.IsActivated)
				{
					IsAntiPassBackActivated = result.Result.IsActivated;
					AntiPassBackHasChanged = true;
				}
				if (SelectedAntiPassBackMode != result.Result.CurrentAntiPassBackMode)
				{
					SelectedAntiPassBackMode = result.Result.CurrentAntiPassBackMode;
					AntiPassBackHasChanged = true;
				}
			}
		}
		void SetAntiPassBack()
		{
			if (!AntiPassBackHasChanged)
				return;

			var result = FiresecManager.FiresecService.SKDSetAntiPassBackConfiguration(
				DeviceViewModel.Device,
				new SKDAntiPassBackConfiguration() { IsActivated = IsAntiPassBackActivated , CurrentAntiPassBackMode = SelectedAntiPassBackMode });
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}

			AntiPassBackHasChanged = false;
		}

		void GetInterlock()
		{
			var result = FiresecManager.FiresecService.SKDGetInterlockConfiguration(DeviceViewModel.Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				if (IsInterlockActivated != result.Result.IsActivated)
				{
					IsInterlockActivated = result.Result.IsActivated;
					InterlockHasChanged = true;
				}
				if (SelectedInterlockMode != result.Result.CurrentInterlockMode)
				{
					SelectedInterlockMode = result.Result.CurrentInterlockMode;
					InterlockHasChanged = true;
				}
			}
		}
		void SetInterlock()
		{
			if (!InterlockHasChanged)
				return;

			var result = FiresecManager.FiresecService.SKDSetInterlockConfiguration(
				DeviceViewModel.Device,
				new SKDInterlockConfiguration() { IsActivated = IsInterlockActivated, CurrentInterlockMode = SelectedInterlockMode });
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}

			InterlockHasChanged = false;
		}

		public RelayCommand ReadFromControllerCommand { get; private set; }
		void OnReadFromController()
		{
			GetDoorType();
			GetAntiPassBack();
			GetInterlock();
		}
		
		public RelayCommand WriteToControllerCommand { get; private set; }
		void OnWriteToController()
		{
			SetAntiPassBack();
			SetInterlock();
			
			// Выполняется последней, т.к. при этой операции происходит перезагрузка контроллера
			if (SetDoorType())
				MessageBoxService.Show("Выполняется перезагрузка контроллера. Контроллер будет доступен через несколько секунд");
		}

		protected override bool Save()
		{
			if (HasChanged)
			{
				if (MessageBoxService.ShowConfirmation(Resources.SaveConfigurationControllerWarning))
				{
					DeviceViewModel.Device.DoorType = SelectedDoorType;
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}

			Close(false);
			return false;
		}
	}
}