using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common;
using FiresecClient;
using StrazhAPI.SKD;
using System.Collections.ObjectModel;

namespace StrazhModule.ViewModels
{
	public class ControllerTimeSettingsViewModel : SaveCancelDialogViewModel
	{
		DeviceViewModel DeviceViewModel { get; set; }
		bool HasChanged { get; set; }

		public ControllerTimeSettingsViewModel(DeviceViewModel deviceViewModel)
		{
			Title = "Временные настройки контроллера";
			DeviceViewModel = deviceViewModel;

			SynchronizeDeviceTimeCommand = new RelayCommand(OnSynchronizeDeviceTime);
			WriteCommand = new RelayCommand(OnWrite);
			ReadCommand = new RelayCommand(OnRead);

			AvailableTimeZoneTypes = new ObservableCollection<SKDTimeZoneType>(Enum.GetValues(typeof(SKDTimeZoneType)).Cast<SKDTimeZoneType>());
			OnRead();
			GetDeviceTime();
			HasChanged = false;
		}

		bool _isEnabled;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				OnPropertyChanged(() => IsEnabled);
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

		string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		int _port;
		public int Port
		{
			get { return _port; }
			set
			{
				_port = value;
				OnPropertyChanged(() => Port);
			}
		}

		int _updatePeriod;
		public int UpdatePeriod
		{
			get { return _updatePeriod; }
			set
			{
				_updatePeriod = value;
				OnPropertyChanged(() => UpdatePeriod);
			}
		}

		ObservableCollection<SKDTimeZoneType> _availableTimeZoneTypes;
		public ObservableCollection<SKDTimeZoneType> AvailableTimeZoneTypes
		{
			get { return _availableTimeZoneTypes; }
			set
			{
				_availableTimeZoneTypes = value;
				OnPropertyChanged(() => AvailableTimeZoneTypes);
			}
		}

		SKDTimeZoneType _selectedTimeZoneType;
		public SKDTimeZoneType SelectedTimeZoneType
		{
			get { return _selectedTimeZoneType; }
			set
			{
				_selectedTimeZoneType = value;
				OnPropertyChanged(() => SelectedTimeZoneType);
			}
		}

		private DateTime _deviceTime;
		public DateTime DeviceTime
		{
			get { return _deviceTime; }
			set
			{
				if (_deviceTime == value)
					return;
				_deviceTime = value;
				OnPropertyChanged(() => DeviceTime);
			}
		}

		private void GetDeviceTime()
		{
			var result = FiresecManager.FiresecService.SKDGetDeviceInfo(DeviceViewModel.Device);
			if (result.HasError)
				return;
			DeviceTime = result.Result.CurrentDateTime;
		}

		public RelayCommand SynchronizeDeviceTimeCommand { get; private set; }
		private void OnSynchronizeDeviceTime()
		{
			var result = FiresecManager.FiresecService.SKDSynchronizeTime(DeviceViewModel.Device);
			if (result.Result)
			{
				GetDeviceTime();
				MessageBoxService.Show("Операция синхронизации времени завершилась успешно");
			}
			else
			{
				MessageBoxService.ShowWarning("Ошибка во время операции синхронизации времени");
			}
		}

		public RelayCommand WriteCommand { get; private set; }
		void OnWrite()
		{
			var controllerTimeSettings = new SKDControllerTimeSettings();
			controllerTimeSettings.IsEnabled = IsEnabled;
			controllerTimeSettings.Name = Name;
			controllerTimeSettings.Description = Description;
			controllerTimeSettings.Port = Port;
			controllerTimeSettings.UpdatePeriod = UpdatePeriod;
			controllerTimeSettings.TimeZone = SelectedTimeZoneType;

			var result = FiresecManager.FiresecService.SetControllerTimeSettings(DeviceViewModel.Device, controllerTimeSettings);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				HasChanged = true;
			}
		}

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			var result = FiresecManager.FiresecService.GetControllerTimeSettings(DeviceViewModel.Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			else
			{
				var controllerTimeSettings = result.Result;
				IsEnabled = controllerTimeSettings.IsEnabled;
				Name = controllerTimeSettings.Name;
				Description = controllerTimeSettings.Description;
				Port = controllerTimeSettings.Port;
				UpdatePeriod = controllerTimeSettings.UpdatePeriod;
				SelectedTimeZoneType = controllerTimeSettings.TimeZone;
				HasChanged = true;
			}
		}

		protected override bool Save()
		{
			return base.Save();
		}
	}
}