using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Microsoft.Win32;
using XFiresecAPI;

namespace SKDModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		DevicesViewModel DevicesViewModel;

		public DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
		{
			DevicesViewModel = devicesViewModel;

            WriteConfigCommand = new RelayCommand(OnWriteConfig, CanWriteConfig);
			ShowInfoCommand = new RelayCommand(OnShowInfo, CanShowInfo);
			SynchroniseTimeCommand = new RelayCommand(OnSynchroniseTime, CanSynchroniseTime);
			UpdateFirmwhareCommand = new RelayCommand(OnUpdateFirmwhare, CanUpdateFirmwhare);
		}

		public DeviceViewModel SelectedDevice
		{
			get { return DevicesViewModel.SelectedDevice; }
		}

		public RelayCommand ShowInfoCommand { get; private set; }
		void OnShowInfo()
		{
			var result = FiresecManager.FiresecService.SKDGetDeviceInfo(SelectedDevice.Device);
			MessageBoxService.Show(result.Result);
		}
		bool CanShowInfo()
		{
			return SelectedDevice != null && SelectedDevice.Device.DriverType == SKDDriverType.Controller;
		}

		public RelayCommand SynchroniseTimeCommand { get; private set; }
		void OnSynchroniseTime()
		{
			var result = FiresecManager.FiresecService.SKDSyncronyseTime(SelectedDevice.Device);
			if (result.Result)
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
			return SelectedDevice != null && SelectedDevice.Device.DriverType == SKDDriverType.Controller;
		}

		public RelayCommand WriteConfigCommand { get; private set; }
		void OnWriteConfig()
		{
			if (CheckNeedSave(true))
			{
				if (ValidateConfiguration())
				{
					var result = FiresecManager.FiresecService.SKDWriteConfiguration(SelectedDevice.Device);
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
				SelectedDevice != null && SelectedDevice.Device.DriverType == SKDDriverType.Controller;
        }

		public RelayCommand UpdateFirmwhareCommand { get; private set; }
		void OnUpdateFirmwhare()
		{
		    var result = new OperationResult<bool>();
			if (SelectedDevice.Device.DriverType == SKDDriverType.Controller)
		    {
		        var openDialog = new OpenFileDialog()
		        {
		            Filter = "FSCS updater|*.fscs",
		            DefaultExt = "FSCS updater|*.fscs"
		        };
		        if (openDialog.ShowDialog().Value)
		        {
                    //var skdControllerDevice = XManager.DeviceConfiguration.Devices.FindAll(x => (x.Driver.DriverType == SKDDriverType.Controller));
					//var firmWareUpdateViewModel = new FirmWareUpdateViewModel(skdControllerDevice);
					//if (DialogService.ShowModalWindow(firmWareUpdateViewModel))
					//{
					//    var hxcFileInfo = HXCFileInfoHelper.Load(openDialog.FileName);
					//    var devices = new List<XDevice>();
					//    firmWareUpdateViewModel.UpdatedDevices.FindAll(x => x.IsChecked).ForEach(x => devices.Add(x.Device));
					//    result = FiresecManager.FiresecService.SKDUpdateFirmwareFSCS(hxcFileInfo, devices);

					//}
		        }
		    }
		    else
            {
		        var openDialog = new OpenFileDialog()
		        {
		            Filter = "soft update files|*.hcs",
		            DefaultExt = "soft update files|*.hcs"
		        };
				if (openDialog.ShowDialog().Value)
				    result = FiresecManager.FiresecService.SKDUpdateFirmware(SelectedDevice.Device, openDialog.FileName);
            }
            if (result.HasError)
                MessageBoxService.ShowError(result.Error, "Ошибка при обновление ПО");
		}

		bool CanUpdateFirmwhare()
		{
			return SelectedDevice != null && SelectedDevice.Driver.DriverType == SKDDriverType.Controller;
		}

		bool ValidateConfiguration()
		{
			var validationResult = ServiceFactory.ValidationService.Validate();
			if (validationResult.HasErrors("SKD"))
			{
				if (validationResult.CannotSave("SKD") || validationResult.CannotWrite("SKD"))
				{
					MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
					return false;
				}
			}
			return true;
		}

		bool CheckNeedSave(bool syncParameters = false)
		{
			if (ServiceFactory.SaveService.SKDChanged)
			{
				if (MessageBoxService.ShowQuestion("Для выполнения этой операции необходимо применить конфигурацию. Применить сейчас?") == System.Windows.MessageBoxResult.Yes)
				{
					//if (syncParameters)
					//    SelectedDevice.SyncFromSystemToDeviceProperties(SelectedDevice.GetRealChildren());
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