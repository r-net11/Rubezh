using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;
using ServerFS2.ConfigurationWriter;
using ServerFS2.Processor;

namespace ClientFS2.ViewModels
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
			ResetFireCommand = new RelayCommand(OnResetFire, CanGetResetDeviceStatus);
			ResetTestCommand = new RelayCommand(OnResetTest, CanGetResetDeviceStatus);
			GetDeviceStatusCommand = new RelayCommand(OnGetDeviceStatus, CanGetResetDeviceStatus);
			AddDeviceToCheckListCommand = new RelayCommand(OnAddDeviceToCheckList, CanAddOrRemoveDeviceToCheckList);
			RemoveDeviceFromCheckListCommand = new RelayCommand(OnRemoveDeviceFromCheckList, CanAddOrRemoveDeviceToCheckList);
			DevicesViewModel = new DevicesViewModel();
			ZonesViewModel = new ZonesViewModel();
			ZonesViewModel.Initialize();
			new PropertiesViewModel(DevicesViewModel);
		}

		private List<byte> status;
		List<byte> Status
		{
			get { return status; }
			set
			{
				status = value;
				if (status != null)
					StatusString = ServerHelper.TraceBytes(value);
				else
					StatusString = "Нет сведений о статусе прибора";
			}
		}
		private string statusString;
		public string StatusString
		{
			get { return statusString; }
			set
			{
				statusString = value;
				OnPropertyChanged("StatusString");
			}
		}
		public bool IsUsbDevice
		{
			get { return ServerHelper.IsUsbDevice; }
			set
			{
				ServerHelper.IsUsbDevice = value;
				OnPropertyChanged("IsUsbDevice");
			}
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
			var autoDetectedDevicesViewModel = new DevicesViewModel(DevicesViewModel.SelectedDevice.Device);
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
			return DeviceValidation(DevicesViewModel.SelectedDevice, false);
		}

		public RelayCommand ReadConfigurationCommand { get; private set; }
		private void OnReadConfiguration()
		{
			var remoteDeviceConfiguration = new DeviceConfiguration();
			_progressService.Run(() =>
			{
				remoteDeviceConfiguration = ServerHelper.GetDeviceConfig(DevicesViewModel.SelectedDevice.Device);
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
			var configurationWriterHelper = new SystemDatabaseCreator();
			configurationWriterHelper.Run();
			var panelDatabase = configurationWriterHelper.PanelDatabases.FirstOrDefault(x => x.ParentPanel.IntAddress == DevicesViewModel.SelectedDevice.Device.IntAddress);
			var bytes1 = panelDatabase.RomDatabase.BytesDatabase.GetBytes();
			var bytes2 = panelDatabase.FlashDatabase.BytesDatabase.GetBytes();
			ServerHelper.SetDeviceConfig(DevicesViewModel.SelectedDevice.Device, bytes2, bytes1);

		}
		bool CanWriteConfiguration()
		{
			return DevicesViewModel.SelectedDevice != null;
		}

		public RelayCommand ResetFireCommand { get; private set; }
		private void OnResetFire()
		{
			ServerHelper.ResetFire(DevicesViewModel.SelectedDevice.Device);
		}

		public RelayCommand ResetTestCommand { get; private set; }
		private void OnResetTest()
		{
			OnGetDeviceStatus();
			if (Status == null)
				return;
			ServerHelper.ResetTest(DevicesViewModel.SelectedDevice.Device, Status);
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