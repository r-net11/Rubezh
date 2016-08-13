using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using StrazhAPI;
using StrazhAPI.SKD;
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
		private DeviceViewModel DeviceViewModel { get; set; }

		public ControllerDoorTypeViewModel(DeviceViewModel deviceViewModel)
		{
			Title = "Основные настройки контроллера";
			DeviceViewModel = deviceViewModel;

			ReadDoorTypeFromControllerCommand = new RelayCommand(OnReadDoorTypeFromController);
			WriteDoorTypeToControllerCommand = new RelayCommand(OnWriteDoorTypeToController);
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

		private bool NeedSaveDoorTypeToController { get; set; }
		private bool NeedSaveAntipassbackToController { get; set; }
		private bool NeedSaveInterlockToController { get; set; }

		bool NeedSaveChangesToController
		{
			get { return NeedSaveDoorTypeToController || NeedSaveAntipassbackToController || NeedSaveInterlockToController; }
			set
			{
				NeedSaveDoorTypeToController = value;
				NeedSaveAntipassbackToController = value;
				NeedSaveInterlockToController = value;
			}
		}

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
				NeedSaveDoorTypeToController = true;
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
				NeedSaveAntipassbackToController = true;
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
				NeedSaveAntipassbackToController = true;
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
				NeedSaveInterlockToController = true;
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
				NeedSaveInterlockToController = true;
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

		private OperationResult<DoorType> GetDoorType()
		{
			var result = FiresecManager.FiresecService.GetControllerDoorType(DeviceViewModel.Device);
			if (!result.HasError)
				SelectedDoorType = result.Result;
			return result;
		}
		private OperationResult<bool> SetDoorType()
		{
			//if (!SelectedDoorTypeHasChanged)
			//	return true;

			return FiresecManager.FiresecService.SetControllerDoorType(DeviceViewModel.Device, SelectedDoorType);
		}

		private OperationResult<SKDAntiPassBackConfiguration> GetAntiPassBack()
		{
			var result = FiresecManager.FiresecService.SKDGetAntiPassBackConfiguration(DeviceViewModel.Device);
			if (!result.HasError)
			{
				IsAntiPassBackActivated = result.Result.IsActivated;
				SelectedAntiPassBackMode = result.Result.CurrentAntiPassBackMode;
			}
			return result;
		}
		private OperationResult<bool> SetAntiPassBack()
		{
			//if (!AntiPassBackHasChanged)
			//	return true;

			return FiresecManager.FiresecService.SKDSetAntiPassBackConfiguration(
				DeviceViewModel.Device,
				new SKDAntiPassBackConfiguration() { IsActivated = IsAntiPassBackActivated , CurrentAntiPassBackMode = SelectedAntiPassBackMode });
		}

		private OperationResult<SKDInterlockConfiguration> GetInterlock()
		{
			var result = FiresecManager.FiresecService.SKDGetInterlockConfiguration(DeviceViewModel.Device);
			if (!result.HasError)
			{
				IsInterlockActivated = result.Result.IsActivated;
				SelectedInterlockMode = result.Result.CurrentInterlockMode;
			}
			return result;
		}
		private OperationResult<bool> SetInterlock()
		{
			//if (!InterlockHasChanged)
			//	return true;

			return FiresecManager.FiresecService.SKDSetInterlockConfiguration(
				DeviceViewModel.Device,
				new SKDInterlockConfiguration() { IsActivated = IsInterlockActivated, CurrentInterlockMode = SelectedInterlockMode });
		}

		public RelayCommand ReadDoorTypeFromControllerCommand { get; private set; }
		private void OnReadDoorTypeFromController()
		{
			var result = GetDoorType();
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			NeedSaveDoorTypeToController = false;
		}

		public RelayCommand WriteDoorTypeToControllerCommand { get; private set; }
		private void OnWriteDoorTypeToController()
		{
			var result = SetDoorType();
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			//if (SelectedDoorTypeHasChanged)
			//	MessageBoxService.Show("Выполняется перезагрузка контроллера. Контроллер будет доступен через несколько секунд");

			NeedSaveDoorTypeToController = false;
		}

		public RelayCommand ReadFromControllerCommand { get; private set; }
		private void OnReadFromController()
		{
			var errors = new HashSet<string>();
			
			var antipassbackResult = GetAntiPassBack();
			if (antipassbackResult.HasError)
				antipassbackResult.Errors.ForEach(error => errors.Add(error));
			
			var interlockResult = GetInterlock();
			if (interlockResult.HasError)
				interlockResult.Errors.ForEach(error => errors.Add(error));
			
			if (errors.Count > 0)
			{
				MessageBoxService.ShowWarning(String.Join("\n", errors));
				return;
			}
			
			NeedSaveAntipassbackToController = false;
			NeedSaveInterlockToController = false;
		}
		
		public RelayCommand WriteToControllerCommand { get; private set; }
		private void OnWriteToController()
		{
			var errors = new HashSet<string>();

			var antipassbackResult = SetAntiPassBack();
			if (antipassbackResult.HasError)
				antipassbackResult.Errors.ForEach(error => errors.Add(error));

			var interlockResult = SetInterlock();
			if (interlockResult.HasError)
				interlockResult.Errors.ForEach(error => errors.Add(error));

			if (errors.Count > 0)
			{
				MessageBoxService.ShowWarning(String.Join("\n", errors));
				return;
			}

			NeedSaveAntipassbackToController = false;
			NeedSaveInterlockToController = false;
		}

		protected override bool Save()
		{
			if (HasChanged)
			{
				DeviceViewModel.Device.DoorType = SelectedDoorType;
				DeviceViewModel.Device.AntiPassBackConfiguration = new SKDAntiPassBackConfiguration { IsActivated = IsAntiPassBackActivated, CurrentAntiPassBackMode = SelectedAntiPassBackMode };
				UpdateLocksConfiguration();
				UpdateReaders();
				DeviceViewModel.Device.InterlockConfiguration = new SKDInterlockConfiguration { IsActivated = IsInterlockActivated, CurrentInterlockMode = SelectedInterlockMode };
				ServiceFactory.SaveService.SKDChanged = true;
			}
			if (NeedSaveChangesToController && !MessageBoxService.ShowConfirmation(Resources.SaveConfigurationControllerWarning))
			{
				return false;
			}
			
			DeviceViewModel.UpdateProperties();
			return base.Save();
		}

		/// <summary>
		/// Активирует/деактивирует свойство IsRepeatEnterAlarmEnable для замков контроллера согласно текущим настройкам AntiPassBack
		/// </summary>
		private void UpdateLocksConfiguration()
		{
			var locks = DeviceViewModel.Device.Children.Where(c => c.DriverType == SKDDriverType.Lock && c.IsEnabled);
			foreach (var lockDevice in locks)
			{
				SKDManager.InvalidateOneLockConfiguration(lockDevice);
				lockDevice.SKDDoorConfiguration.IsRepeatEnterAlarmEnable = IsAntiPassBackActivated;
			}
		}

		private void UpdateReaders()
		{
			if (DeviceViewModel.Device.DriverType == SKDDriverType.ChinaController_1 &&
			    DeviceViewModel.Device.DoorType == DoorType.OneWay)
			{
				var readers = DeviceViewModel.Children.Where(x => x.Device.DriverType == SKDDriverType.Reader && x.Device.IntAddress > 0);
				foreach (var reader in readers)
				{
					SKDManager.RemoveDeviceFromZone(reader.Device);
				}
			}
		}
	}
}