using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using Infrastructure.Common;
using FiresecClient;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class ControllerTimeSettingsViewModel : SaveCancelDialogViewModel
	{
		DeviceViewModel DeviceViewModel { get; set; }
		bool HasChanged { get; set; }

		public ControllerTimeSettingsViewModel(DeviceViewModel deviceViewModel)
		{
			Title = "Синхронизация времени контроллера";
			DeviceViewModel = deviceViewModel;

			WriteCommand = new RelayCommand(OnWrite);
			ReadCommand = new RelayCommand(OnRead);
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

		SKDTimeZoneType _timeZone;
		public SKDTimeZoneType TimeZone
		{
			get { return _timeZone; }
			set
			{
				_timeZone = value;
				OnPropertyChanged(() => TimeZone);
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
			controllerTimeSettings.TimeZone = TimeZone;

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
				TimeZone = controllerTimeSettings.TimeZone;
				HasChanged = true;
			}
		}

		protected override bool Save()
		{
			return base.Save();
		}
	}
}