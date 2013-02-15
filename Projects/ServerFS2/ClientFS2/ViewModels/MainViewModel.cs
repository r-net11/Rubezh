using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;

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
			SetParametersCommand = new RelayCommand(OnSetParameters, CanSetParameters);
			GetParametersCommand = new RelayCommand(OnGetParameters, CanGetParameters);
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
			if (DevicesViewModel.SelectedDevice == null)
				return false;

			switch (DevicesViewModel.SelectedDevice.Driver.DriverType)
			{
				case DriverType.USB_Rubezh_2AM:
				case DriverType.USB_Rubezh_4A:
				case DriverType.USB_Rubezh_2OP:
				case DriverType.USB_BUNS:
				case DriverType.USB_BUNS_2:
				case DriverType.Rubezh_2AM:
				case DriverType.BUNS:
				case DriverType.BUNS_2:
				case DriverType.Rubezh_10AM:
				case DriverType.Rubezh_4A:
				case DriverType.Rubezh_2OP:
					return true;
			}
			return false;
		}

		public RelayCommand ReadConfigurationCommand { get; private set; }
		private void OnReadConfiguration()
		{

		}
		bool CanReadConfiguration()
		{
			return false;
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

		}
		bool CanSynchronizeTime()
		{
			return false;
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

		public RelayCommand GetParametersCommand { get; private set; }
		private void OnGetParameters()
		{
            //var properties = new List<Property>();
            //_progressService.Run(() =>{properties = ServerHelper.GetDeviceParameters(DevicesViewModel.SelectedDevice.Device);}, 
            //() => DialogService.ShowModalWindow(new PropertiesViewModel(DevicesViewModel)), "Получение параметров устройства");
		}
		bool CanGetParameters()
		{
			return DevicesViewModel.SelectedDevice != null;
		}

		public RelayCommand SetParametersCommand { get; private set; }
		private void OnSetParameters()
		{
			
		}
		bool CanSetParameters()
		{
			return false;
		}
	}
}