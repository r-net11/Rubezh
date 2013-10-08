using System;
using System.Collections.Generic;
using System.Windows;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Microsoft.Win32;
using XFiresecAPI;

namespace GKModule.Models
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		DevicesViewModel _devicesViewModel;

		public DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
		{
			ReadConfigurationCommand = new RelayCommand(OnReadConfiguration, CanReadConfiguration);
            WriteConfigCommand = new RelayCommand(OnWriteConfig, CanWriteConfig);

			ShowInfoCommand = new RelayCommand(OnShowInfo, CanShowInfo);
			SynchroniseTimeCommand = new RelayCommand(OnSynchroniseTime, CanSynchroniseTime);
			ReadJournalCommand = new RelayCommand(OnReadJournal, CanReadJournal);
			GetAllParametersCommand = new RelayCommand(OnGetAllParameters);
			SetAllParametersCommand = new RelayCommand(OnSetAllParameters, CanSetAllParameters);
            GetSingleParameterCommand = new RelayCommand(OnGetSingleParameter, CanGetSetSingleParameter);
            SetSingleParameterCommand = new RelayCommand(OnSetSingleParameter, CanSetSingleParameter);
			UpdateFirmwhareCommand = new RelayCommand(OnUpdateFirmwhare, CanUpdateFirmwhare);

			_devicesViewModel = devicesViewModel;
		}

		public DeviceViewModel SelectedDevice
		{
			get { return _devicesViewModel.SelectedDevice; }
		}

		public RelayCommand ShowInfoCommand { get; private set; }
		void OnShowInfo()
		{
			var result = DeviceBytesHelper.GetDeviceInfo(SelectedDevice.Device);
			if (!string.IsNullOrEmpty(result))
			{
				MessageBoxService.Show(result);
			}
			else
			{
				MessageBoxService.Show("Ошибка при запросе информации об устройстве");
			}
		}
		bool CanShowInfo()
		{
			return (SelectedDevice != null && (SelectedDevice.Device.Driver.IsKauOrRSR2Kau || SelectedDevice.Device.Driver.DriverType == XDriverType.GK));
		}

		public RelayCommand SynchroniseTimeCommand { get; private set; }
		void OnSynchroniseTime()
		{
			var result = DeviceBytesHelper.WriteDateTime(SelectedDevice.Device);
			if (result)
			{
				MessageBoxService.Show("Операция синхронизации времени завершилась успешно");
			}
			else
			{
				MessageBoxService.Show("Ошибка во время операции синхронизации времени");
			}
		}
		bool CanSynchroniseTime()
		{
			return (SelectedDevice != null && SelectedDevice.Device.Driver.DriverType == XDriverType.GK);
		}

		public RelayCommand ReadJournalCommand { get; private set; }
		void OnReadJournal()
		{
			var journalViewModel = new JournalViewModel(SelectedDevice.Device);
			if (journalViewModel.Initialize())
			{
				DialogService.ShowModalWindow(journalViewModel);
			}
		}
		bool CanReadJournal()
		{
			return (SelectedDevice != null && SelectedDevice.Device.Driver.DriverType == XDriverType.GK);
		}

		public RelayCommand WriteConfigCommand { get; private set; }
		void OnWriteConfig()
		{
			if (ValidateConfiguration())
			{
				var dateTime = DateTime.Now;
				GKDBHelper.AddMessage("Запись конфигурации в прибор", FiresecManager.CurrentUser.Name);
				BinConfigurationWriter.WriteConfig(SelectedDevice.Device);
#if DEBUG
				MessageBoxService.Show("Время записи, мин " + (DateTime.Now - dateTime).TotalMinutes);
#endif
			}
		}
        bool CanWriteConfig()
        {
			return FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig) &&
				SelectedDevice != null &&
				SelectedDevice.Device.Driver.DriverType == XDriverType.GK;
        }

		public RelayCommand ReadConfigurationCommand { get; private set; }
		void OnReadConfiguration()
		{
			var device = SelectedDevice.Device;

			var sendResult = SendManager.Send(device, 0, 1, 1);
			if (sendResult.HasError)
			{
				MessageBoxService.ShowError("Устройство " + device.PresentationDriverAndAddress + " недоступно");
				return;
			}

			if (device.Driver.IsKauOrRSR2Kau)
			{
				var remoteDevices = KauBinConfigurationReader.ReadConfiguration(device);
				var deviceConfigurationViewModel = new DeviceConfigurationViewModel(device, remoteDevices);
				if (DialogService.ShowModalWindow(deviceConfigurationViewModel))
				{
					XManager.UpdateConfiguration();
					SelectedDevice.CollapseChildren();
					SelectedDevice.ClearChildren();
					foreach (var remoteDevice in remoteDevices)
					{
						DevicesViewModel.Current.AddDevice(remoteDevice, SelectedDevice);
					}
					SelectedDevice.ExpandChildren();
				}
			}
			if (device.Driver.DriverType == XDriverType.GK)
			{
				if (MessageBoxService.ShowQuestion("Текущая конфигурация будет заменена считанной из устройства. При этом часть данных возможно будет потеряня. Продолжить?") != MessageBoxResult.Yes)
					return;

				var gkBinConfigurationReader = new GkBinConfigurationReader();
				gkBinConfigurationReader.ReadConfiguration(device);

				XManager.DeviceConfiguration = gkBinConfigurationReader.DeviceConfiguration;
				ServiceFactory.SaveService.FSChanged = true;
				ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
			}
		}
		bool CanReadConfiguration()
		{
			return (SelectedDevice != null && (SelectedDevice.Device.Driver.IsKauOrRSR2Kau));
		}

		public RelayCommand GetAllParametersCommand { get; private set; }
		void OnGetAllParameters()
		{
			ParametersHelper.GetAllParameters();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand SetAllParametersCommand { get; private set; }
		void OnSetAllParameters()
		{
			ParametersHelper.SetAllParameters();
		}
		bool CanSetAllParameters()
		{
			return CanGetSetSingleParameter() && FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig);
		}

		public RelayCommand GetSingleParameterCommand { get; private set; }
		void OnGetSingleParameter()
		{
			DatabaseManager.Convert();
			ParametersHelper.GetSingleParameter(SelectedDevice.Device);
			ServiceFactory.SaveService.GKChanged = true;
		}

        public RelayCommand SetSingleParameterCommand { get; private set; }
        void OnSetSingleParameter()
		{
			DatabaseManager.Convert();
            ParametersHelper.SetSingleParameter(SelectedDevice.Device);
		}
		bool CanSetSingleParameter()
		{
			return CanGetSetSingleParameter() && FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig);
		}

        bool CanGetSetSingleParameter()
        {
            if(SelectedDevice == null)
                return false;
            if(SelectedDevice.Device.Driver.DriverType == XDriverType.System)
                return false;
            if(SelectedDevice.Device.Driver.IsGroupDevice)
                return false;
            return true;
        }

		public RelayCommand UpdateFirmwhareCommand { get; private set; }
		void OnUpdateFirmwhare()
		{
			var updateFilePath = ChangeFile();
			if (updateFilePath == null)
				return;
			var firmWareBytes = HexHelper.HexFileToBytesList(updateFilePath);
			var selectedDevice = SelectedDevice.Device;
			BinConfigurationWriter.GoToTechnologicalRegime(selectedDevice);
			var softVersion = DeviceBytesHelper.GetDeviceInfo(selectedDevice);
			BinConfigurationWriter.Clear(selectedDevice);
			var data = new List<byte>();
			for (int i = 0; i < firmWareBytes.Count; i = i + 0x100)
			{
				data = new List<byte>(BitConverter.GetBytes((i + 1)*0x100));
				data.Reverse();
				data.AddRange(firmWareBytes.GetRange(i, 0x100));
				SendManager.Send(selectedDevice, 260, 0x12, 0, data);
			}
		}
		bool CanUpdateFirmwhare()
		{
			return (SelectedDevice != null && (SelectedDevice.Driver.IsKauOrRSR2Kau || SelectedDevice.Driver.DriverType == XDriverType.GK) && FiresecManager.CheckPermission(PermissionType.Adm_ChangeDevicesSoft));
		}

		static string ChangeFile()
		{
			var openDialog = new OpenFileDialog()
			{
				Filter = "soft update files|*.hcs",
				DefaultExt = "soft update files|*.hcs"
			};
			if (openDialog.ShowDialog().Value)
				return openDialog.FileName;
			return null;
		}

		bool ValidateConfiguration()
		{
			var validationResult = ServiceFactory.ValidationService.Validate();
			if (validationResult.HasErrors("GK"))
			{
				if (validationResult.CannotSave("GK") || validationResult.CannotWrite("GK"))
				{
					MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
					return false;
				}
			}
			return true;
		}
	}
}