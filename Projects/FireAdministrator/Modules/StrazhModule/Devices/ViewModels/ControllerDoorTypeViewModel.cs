using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using FiresecAPI.SKD;
using FiresecClient;

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

			HasChanged = false;
		}

		#region Флаги изменения настроек контроллера

		bool SelectedDoorTypeHasChanged { get; set; }
		bool AntiPassBackHasChanged { get; set; }
		bool HasChanged
		{
			get
			{
				return SelectedDoorTypeHasChanged || AntiPassBackHasChanged;
			}
			set
			{
				SelectedDoorTypeHasChanged = value;
				AntiPassBackHasChanged = value;
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
		void SetDoorType()
		{
			if (!SelectedDoorTypeHasChanged)
				return;

			var result = FiresecManager.FiresecService.SetControllerDoorType(DeviceViewModel.Device, SelectedDoorType);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}

			SelectedDoorTypeHasChanged = false;
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

		public RelayCommand ReadFromControllerCommand { get; private set; }
		void OnReadFromController()
		{
			GetDoorType();
			GetAntiPassBack();
		}
		
		public RelayCommand WriteToControllerCommand { get; private set; }
		void OnWriteToController()
		{
			SetAntiPassBack();
			
			// Выполняется последней, т.к. при этой операции происходит перезагрузка контроллера
			SetDoorType();
		}

		protected override bool Save()
		{
			if (!HasChanged)
			{
				DeviceViewModel.Device.DoorType = SelectedDoorType;
				ServiceFactory.SaveService.SKDChanged = true;
				return base.Save();
			}

			if (!MessageBoxService.ShowConfirmation(
				"Настройки не записаны в прибор. Вы уверены, что хотите закрыть окно без записи в контроллер?")) return false;
			HasChanged = false;
			Close(false);

			return false;
		}
	}
}