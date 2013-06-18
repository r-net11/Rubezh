using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;
using ServerFS2.ConfigurationWriter;
using ServerFS2.Processor;
using System.Diagnostics;

namespace AdministratorTestClientFS2.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		public DevicesViewModel DevicesViewModel { get; private set; }
		public ZonesViewModel ZonesViewModel { get; private set; }
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
			GetDeviceStatusCommand = new RelayCommand(OnGetDeviceStatus, CanGetResetDeviceStatus);
			AddDeviceToCheckListCommand = new RelayCommand(OnAddDeviceToCheckList, CanAddOrRemoveDeviceToCheckList);
			RemoveDeviceFromCheckListCommand = new RelayCommand(OnRemoveDeviceFromCheckList, CanAddOrRemoveDeviceToCheckList);
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			ZonesViewModel.Initialize();
			new PropertiesViewModel(DevicesViewModel);
		}

		List<byte> _status;
		List<byte> Status
		{
			get { return _status; }
			set
			{
				_status = value;
				if (_status != null)
					StatusString = TraceHelper.TraceBytes(value);
				else
					StatusString = "Нет сведений о статусе прибора";
			}
		}

		string _statusString;
		public string StatusString
		{
			get { return _statusString; }
			set
			{
				_statusString = value;
				OnPropertyChanged("StatusString");
			}
		}

		bool _isUsbDevice;
		public bool IsUsbDevice
		{
			get { return _isUsbDevice; }
			set
			{
				_isUsbDevice = value;
				OnPropertyChanged("IsUsbDevice");
			}
		}

		string _textBoxRequest;
		public string TextBoxRequest
		{
			get { return _textBoxRequest; }
			set
			{
				_textBoxRequest = value;
				OnPropertyChanged("TextBoxRequest");
			}
		}

		string _textBoxResponse;
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
			var bytes = TextBoxRequest.Split().Select(t => byte.Parse(t, NumberStyles.AllowHexSpecifier)).ToList();
			var response = USBManager.SendCodeToPanel(DevicesViewModel.SelectedDevice.Device, bytes);
			TextBoxResponse += BytesHelper.BytesToString(response.Bytes);
		}

		public RelayCommand AutoDetectDeviceCommand { get; private set; }
		private void OnAutoDetectDevice()
		{
			var autoDetectedDevicesViewModel = new DevicesViewModel(DevicesViewModel.SelectedDevice.Device);
			_progressService.Run(() =>
			{
				var device = AutoDetectOperationHelper.AutoDetectDevice();
				autoDetectedDevicesViewModel = new DevicesViewModel(device);
			}, () => DialogService.ShowModalWindow(autoDetectedDevicesViewModel), "Автопоиск устройств");
		}

		public RelayCommand ReadJournalCommand { get; private set; }
		private void OnReadJournal()
		{
			var journalItems = MainManager.DeviceReadEventLog(DevicesViewModel.SelectedDevice.Device, IsUsbDevice);
			var journalViewModel = new JournalViewModel(journalItems);
			DialogService.ShowModalWindow(journalViewModel);
		}
		bool CanReadJournal()
		{
			return DeviceValidation(DevicesViewModel.SelectedDevice, false);
		}

		public RelayCommand ReadConfigurationCommand { get; private set; }
		private void OnReadConfiguration()
		{
			var remoteDeviceConfiguration = new DeviceConfiguration();
			_progressService.Run(() =>
			{
				remoteDeviceConfiguration = MainManager.DeviceReadConfig(DevicesViewModel.SelectedDevice.Device, IsUsbDevice);
			},
			() => DialogService.ShowModalWindow(new DeviceConfigurationViewModel(DevicesViewModel.SelectedDevice.Device.UID, remoteDeviceConfiguration)), "Считывание конфигурации с устройства");

		}
		bool CanReadConfiguration()
		{
			return ((DevicesViewModel.SelectedDevice != null) && (DevicesViewModel.SelectedDevice.Device.Driver.IsPanel));
		}
		public RelayCommand GetInformationCommand { get; private set; }
		private void OnGetInformation()
		{
			var result = MainManager.DeviceGetInformation(DevicesViewModel.SelectedDevice.Device, IsUsbDevice);
			MessageBoxService.Show(result);
		}
		bool CanGetInformation()
		{
			return false;
		}

		public RelayCommand SynchronizeTimeCommand { get; private set; }
		private void OnSynchronizeTime()
		{
			MainManager.DeviceDatetimeSync(DevicesViewModel.SelectedDevice.Device, IsUsbDevice);
		}
		bool CanSynchronizeTime()
		{
			return ((DevicesViewModel.SelectedDevice != null) && (DevicesViewModel.SelectedDevice.Device.Driver.IsPanel));
		}

		public RelayCommand SetPasswordCommand { get; private set; }
		private void OnSetPassword()
		{
			MainManager.DeviceSetPassword(DevicesViewModel.SelectedDevice.Device, IsUsbDevice, DevicePasswordType.Administrator, "123");
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
			MainManager.DeviceWriteConfig(DevicesViewModel.SelectedDevice.Device, IsUsbDevice);

		}
		bool CanWriteConfiguration()
		{
			return DevicesViewModel.SelectedDevice != null;
		}

		public RelayCommand GetDeviceStatusCommand { get; private set; }
		void OnGetDeviceStatus()
		{
			Status = ServerHelper.GetDeviceStatus(DevicesViewModel.SelectedDevice.Device);
		}

		bool CanGetResetDeviceStatus()
		{
			return DeviceValidation(DevicesViewModel.SelectedDevice, false);
		}

		public RelayCommand AddDeviceToCheckListCommand { get; private set; }
		void OnAddDeviceToCheckList()
		{
			MainManager.AddToIgnoreList(new List<Device>() { DevicesViewModel.SelectedDevice.Device });
		}

		bool CanAddOrRemoveDeviceToCheckList()
		{
			return ((DevicesViewModel.SelectedDevice != null) && (DevicesViewModel.SelectedDevice.Device.ParentPanel != null));
		}

		public RelayCommand RemoveDeviceFromCheckListCommand { get; private set; }
		void OnRemoveDeviceFromCheckList()
		{
			MainManager.RemoveFromIgnoreList(new List<Device>() { DevicesViewModel.SelectedDevice.Device });
		}

		bool DeviceValidation(DeviceViewModel selectedDeivice, bool isUsb)
		{
			return (selectedDeivice != null) && (selectedDeivice.Device.Driver.IsPanel) && (selectedDeivice.Device.IsUsb == isUsb);
		}
	}
}