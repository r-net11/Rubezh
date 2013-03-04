using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;
using ClientFS2.ConfigurationWriter;

namespace ClientFS2.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public DevicesViewModel DevicesViewModel { get; private set; }
        private readonly ProgressService _progressService = new ProgressService();
		public MainViewModel()
		{
			SendRequestCommand = new RelayCommand(OnSendRequest);
			AutoDetectDeviceCommand = new RelayCommand(OnAutoDetectDevice);
			ReadConfigurationCommand = new RelayCommand(OnReadConfiguration, CanReadConfiguration);
			ReadJournalCommand = new RelayCommand(OnReadJournal, CanReadJournal);
			GetInformationCommand = new RelayCommand(OnGetInformation, CanGetInformation);
			SynchronizeTimeCommand = new RelayCommand(OnSynchronizeTime, CanSynchronizeTime);
			SetPasswordCommand = new RelayCommand(OnSetPassword, CanSetPassword);
			UpdateFirmwhareCommand = new RelayCommand(OnSynchronizeTime, CanSynchronizeTime);
			UpdateFirmwhareCommand = new RelayCommand(OnUpdateFirmwhare, CanUpdateFirmwhare);
			SetPanelRegimeCommand = new RelayCommand(OnSetPanelRegime, CanSetPanelRegime);
			UnsetPanelRegimeCommand = new RelayCommand(OnUnsetPanelRegime, CanUnsetPanelRegime);
			WriteConfigurationCommand = new RelayCommand(OnWriteConfiguration, CanWriteConfiguration);
			DevicesViewModel = new DevicesViewModel();
            new PropertiesViewModel(DevicesViewModel);
		}

		private string _textBoxRequest;
		public string TextBoxRequest
		{
			get { return _textBoxRequest; }
			set
			{
				_textBoxRequest = value;
				OnPropertyChanged("TextBoxRequest");
			}
		}

		private string _textBoxResponse;
		public string TextBoxResponse
		{
			get { return _textBoxResponse; }
			set
			{
				_textBoxResponse = value;
				OnPropertyChanged("TextBoxResponse");
			}
		}

		public RelayCommand SendRequestCommand { get; private set; }
		private void OnSendRequest()
		{
			var bytes = TextBoxRequest.Split()
				   .Select(t => byte.Parse(t, NumberStyles.AllowHexSpecifier)).ToList();
			var inbytes = ServerHelper.SendRequest(bytes);
			foreach (var b in inbytes)
				TextBoxResponse += b.ToString("X2") + " ";
		}

		public RelayCommand AutoDetectDeviceCommand { get; private set; }
        private void OnAutoDetectDevice()
        {
            var autoDetectedDevicesViewModel = new DevicesViewModel(new Device());
            _progressService.Run(() =>
            {
                var device = ServerHelper.AutoDetectDevice();
                autoDetectedDevicesViewModel = new DevicesViewModel(device);
            }, () => DialogService.ShowModalWindow(autoDetectedDevicesViewModel), "Автопоиск устройств");
        }

		public RelayCommand ReadJournalCommand { get; private set; }
		private void OnReadJournal()
		{
			var journalItems = ServerHelper.GetJournalItems(DevicesViewModel.SelectedDevice.Device);
			var journalViewModel = new JournalViewModel(journalItems);
			DialogService.ShowModalWindow(journalViewModel);
		}
		bool CanReadJournal()
		{
            return ((DevicesViewModel.SelectedDevice != null) && (DevicesViewModel.SelectedDevice.Device.Driver.IsPanel));
		}

		public RelayCommand ReadConfigurationCommand { get; private set; }
		private void OnReadConfiguration()
		{
            var devicesViewModel = new DevicesViewModel(new Device());
            _progressService.Run(() =>
            {
                var device = ServerHelper.GetDeviceConfig(DevicesViewModel.SelectedDevice.Device);
                devicesViewModel = new DevicesViewModel(device);
            }, () => DialogService.ShowModalWindow(devicesViewModel), "Считывание конфигурации с устройства");
		}
		bool CanReadConfiguration()
		{
            return ((DevicesViewModel.SelectedDevice != null) && (DevicesViewModel.SelectedDevice.Device.Driver.IsPanel));
		}
		public RelayCommand GetInformationCommand { get; private set; }
		private void OnGetInformation()
		{

		}
		bool CanGetInformation()
		{
			return false;
		}

		public RelayCommand SynchronizeTimeCommand { get; private set; }
		private void OnSynchronizeTime()
		{
            ServerHelper.SynchronizeTime(DevicesViewModel.SelectedDevice.Device);
		}
		bool CanSynchronizeTime()
		{
            return ((DevicesViewModel.SelectedDevice != null) && (DevicesViewModel.SelectedDevice.Device.Driver.IsPanel));
		}

		public RelayCommand SetPasswordCommand { get; private set; }
		private void OnSetPassword()
		{

		}
		bool CanSetPassword()
		{
			return false;
		}

		public RelayCommand UpdateFirmwhareCommand { get; private set; }
		private void OnUpdateFirmwhare()
		{

		}
		bool CanUpdateFirmwhare()
		{
			return false;
		}

		public RelayCommand SetPanelRegimeCommand { get; private set; }
		private void OnSetPanelRegime()
		{

		}
		bool CanSetPanelRegime()
		{
			return false;
		}

		public RelayCommand UnsetPanelRegimeCommand { get; private set; }
		private void OnUnsetPanelRegime()
		{

		}
		bool CanUnsetPanelRegime()
		{
			return false;
		}

		public RelayCommand WriteConfigurationCommand { get; private set; }
		private void OnWriteConfiguration()
		{
			ConfigurationWriterHelper.Run();
		}
		bool CanWriteConfiguration()
		{
			return true;
		}
	}
}