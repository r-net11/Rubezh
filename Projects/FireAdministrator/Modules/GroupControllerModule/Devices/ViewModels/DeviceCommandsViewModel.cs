using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.ViewModels;
using GKProcessor;
using HexManager;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
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
			_devicesViewModel = devicesViewModel;

			ReadConfigurationCommand = new RelayCommand(OnReadConfiguration, CanReadConfiguration);
			ReadConfigFileCommand = new RelayCommand(OnReadConfigFile, CanReadConfigFile);
            WriteConfigCommand = new RelayCommand(OnWriteConfig, CanWriteConfig);
			ShowInfoCommand = new RelayCommand(OnShowInfo, CanShowInfo);
			SynchroniseTimeCommand = new RelayCommand(OnSynchroniseTime, CanSynchroniseTime);
			ReadJournalCommand = new RelayCommand(OnReadJournal, CanReadJournal);
			UpdateFirmwhareCommand = new RelayCommand(OnUpdateFirmwhare, CanUpdateFirmwhare);
		}

		public DeviceViewModel SelectedDevice
		{
			get { return _devicesViewModel.SelectedDevice; }
		}

		public RelayCommand ShowInfoCommand { get; private set; }
		void OnShowInfo()
		{
			var result = FiresecManager.FiresecService.GKGetDeviceInfo(SelectedDevice.Device);
			MessageBoxService.Show(!string.IsNullOrEmpty(result) ? result : "Ошибка при запросе информации об устройстве");
		}

		bool CanShowInfo()
		{
			return (SelectedDevice != null && (SelectedDevice.Device.Driver.IsKauOrRSR2Kau || SelectedDevice.Device.DriverType == XDriverType.GK));
		}

		public RelayCommand SynchroniseTimeCommand { get; private set; }
		void OnSynchroniseTime()
		{
			var result = FiresecManager.FiresecService.GKSyncronyseTime(SelectedDevice.Device);
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
			return (SelectedDevice != null && SelectedDevice.Device.DriverType == XDriverType.GK);
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
			return (SelectedDevice != null && SelectedDevice.Device.DriverType == XDriverType.GK);
		}

		public RelayCommand WriteConfigCommand { get; private set; }
		void OnWriteConfig()
		{
			if (CheckNeedSave(true))
			{
				if (ValidateConfiguration())
				{
					if (!GlobalSettingsHelper.GlobalSettings.DoNotShowWriteFileToGKDialog)
					{
						var fileWritingViewModel = new FileWritingViewModel();
						DialogService.ShowModalWindow(fileWritingViewModel);
					}
					var result = FiresecManager.FiresecService.GKWriteConfiguration(SelectedDevice.Device, GlobalSettingsHelper.GlobalSettings.WriteFileToGK);
					if (result.HasError)
					{
						MessageBoxService.ShowError(result.Error);
					}
				}
			}
		}

        bool CanWriteConfig()
        {
			return FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig) &&
				SelectedDevice != null &&
				SelectedDevice.Device.DriverType == XDriverType.GK;
        }

		public RelayCommand ReadConfigurationCommand { get; private set; }
		void OnReadConfiguration()
		{
			DescriptorsManager.Create();
			var result = FiresecManager.FiresecService.GKReadConfiguration(SelectedDevice.Device);
			if (!result.HasError)
			{
				XManager.UpdateConfiguration();
				var configurationCompareViewModel = new ConfigurationCompareViewModel(XManager.DeviceConfiguration, result.Result,
					SelectedDevice.Device, false);
				if (DialogService.ShowModalWindow(configurationCompareViewModel))
					ServiceFactoryBase.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
			}
			else
				MessageBoxService.ShowError(result.Error, "Ошибка при чтении конфигурации");
		}

		public RelayCommand ReadConfigFileCommand { get; private set; }
		void OnReadConfigFile()
		{
			DescriptorsManager.Create();
			var gkFileReaderWriter = new GKFileReaderWriter();
			var deviceConfiguration = gkFileReaderWriter.ReadConfigFileFromGK(SelectedDevice.Device);
			if (gkFileReaderWriter.Error == null)
			{
				XManager.UpdateConfiguration();
				var configurationCompareViewModel = new ConfigurationCompareViewModel(XManager.DeviceConfiguration,
					deviceConfiguration, SelectedDevice.Device, true);
				if (DialogService.ShowModalWindow(configurationCompareViewModel))
					ServiceFactoryBase.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
			}
			else
				MessageBoxService.ShowError(gkFileReaderWriter.Error, "Ошибка при чтении конфигурационного файла");
		}

		bool CanReadConfiguration()
		{
			return (SelectedDevice != null && (SelectedDevice.Device.Driver.IsKauOrRSR2Kau || SelectedDevice.Driver.DriverType == XDriverType.GK));
		}

		bool CanReadConfigFile()
		{
			return (SelectedDevice != null && SelectedDevice.Driver.DriverType == XDriverType.GK);
		}

		public RelayCommand UpdateFirmwhareCommand { get; private set; }
		void OnUpdateFirmwhare()
		{
		    if (SelectedDevice.Device.DriverType == XDriverType.System)
		    {
		        var openDialog = new OpenFileDialog()
		        {
		            Filter = "FSCS updater|*.fscs",
		            DefaultExt = "FSCS updater|*.fscs"
		        };
		        if (openDialog.ShowDialog().Value)
		        {
                    var gkKauKauRsr2Devices = XManager.DeviceConfiguration.Devices.FindAll(x => (x.Driver.DriverType == XDriverType.GK)||(x.Driver.IsKauOrRSR2Kau));
                    var firmWareUpdateViewModel = new FirmWareUpdateViewModel(gkKauKauRsr2Devices);
		            if (DialogService.ShowModalWindow(firmWareUpdateViewModel))
		            {
                        var hxcFileInfo = HXCFileInfoHelper.Load(openDialog.FileName);
                        FiresecManager.FiresecService.GKUpdateFirmwareFSCS(hxcFileInfo);  
		            }
		        }
		    }
		    else
            {
		        var openDialog = new OpenFileDialog()
		        {
		            Filter = "soft update files|*.hcs",
		            DefaultExt = "soft update files|*.hcs"
		        };
                FiresecManager.FiresecService.GKUpdateFirmware(SelectedDevice.Device, openDialog.FileName);
			}
		}

		bool CanUpdateFirmwhare()
		{
			return (SelectedDevice != null && (SelectedDevice.Driver.IsKauOrRSR2Kau || SelectedDevice.Driver.DriverType == XDriverType.GK || SelectedDevice.Driver.DriverType == XDriverType.System) && FiresecManager.CheckPermission(PermissionType.Adm_ChangeDevicesSoft));
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

		bool CheckNeedSave(bool syncParameters = false)
		{
			if (ServiceFactory.SaveService.GKChanged)
			{
				if (MessageBoxService.ShowQuestion("Для выполнения этой операции необходимо применить конфигурацию. Применить сейчас?") == System.Windows.MessageBoxResult.Yes)
				{
					if (syncParameters)
						SelectedDevice.SyncFromSystemToDeviceProperties(SelectedDevice.GetRealChildren());
					var cancelEventArgs = new CancelEventArgs();
					ServiceFactory.Events.GetEvent<SetNewConfigurationEvent>().Publish(cancelEventArgs);
					return !cancelEventArgs.Cancel;
				}
				return false;
			}
			return true;
		}
	}
}