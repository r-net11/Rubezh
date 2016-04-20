using GKModule.ViewModels;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Services;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;

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
			//UpdateFirmwareCommand = new RelayCommand(OnUpdateFirmware, CanUpdateFirmware);
			AutoSearchCommand = new RelayCommand(OnAutoSearch, CanAutoSearch);
			GetUsersCommand = new RelayCommand(OnGetUsers, CanGetUsers);
			RewriteUsersCommand = new RelayCommand(OnRewriteUsers, CanRewriteUsers);
			RewriteAllSchedulesCommand = new RelayCommand(OnRewriteAllSchedules, CanRemoveAllSchedules);
		}

		public DeviceViewModel SelectedDevice
		{
			get { return _devicesViewModel.SelectedDevice; }
		}

		public RelayCommand ShowInfoCommand { get; private set; }
		void OnShowInfo()
		{
			var result = ClientManager.FiresecService.GKGetDeviceInfo(SelectedDevice.Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			if (string.IsNullOrEmpty(result.Result))
			{
				MessageBoxService.ShowWarning("Ошибка при запросе информации об устройстве");
				return;
			}
			MessageBoxService.Show(result.Result);
		}

		bool CanShowInfo()
		{
			return (SelectedDevice != null && (SelectedDevice.Device.Driver.IsKau || SelectedDevice.Device.DriverType == GKDriverType.GK || SelectedDevice.Device.DriverType == GKDriverType.GKMirror));
		}

		public RelayCommand SynchroniseTimeCommand { get; private set; }
		void OnSynchroniseTime()
		{
			var result = ClientManager.FiresecService.GKSyncronyseTime(SelectedDevice.Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error);
				return;
			}
			if (!result.Result)
			{
				MessageBoxService.ShowWarning("Ошибка во время операции синхронизации времени");
				return;
			}
			MessageBoxService.Show("Операция синхронизации времени завершилась успешно");
		}
		bool CanSynchroniseTime()
		{
			return (SelectedDevice != null && (SelectedDevice.Device.DriverType == GKDriverType.GK || SelectedDevice.Device.DriverType == GKDriverType.GKMirror));
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
			return (SelectedDevice != null && SelectedDevice.Device.DriverType == GKDriverType.GK);
		}

		public RelayCommand WriteConfigCommand { get; private set; }
		void OnWriteConfig()
		{
			if (CheckNeedSave(true))
			{
				if (ValidateConfiguration())
				{
					var result = ClientManager.FiresecService.GKWriteConfiguration(SelectedDevice.Device.GKParent);
					if (result.HasError)
					{
						MessageBoxService.ShowWarning(result.Error, "Ошибка при записи конфигурации");
					}
				}
			}
		}

		bool CanWriteConfig()
		{
			return ClientManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig) &&
				SelectedDevice != null && SelectedDevice.Device.GKParent != null;
		}

		public RelayCommand ReadConfigurationCommand { get; private set; }
		void OnReadConfiguration()
		{
			var thread = new Thread(() =>
			{
				DescriptorsManager.Create();
				var result = ClientManager.FiresecService.GKReadConfiguration(SelectedDevice.Device);

				ApplicationService.Invoke(new Action(() =>
				{
					if (!result.HasError)
					{
						ConfigurationCompareViewModel configurationCompareViewModel = null;
						WaitHelper.Execute(() =>
						{
							result.Result.UpdateConfiguration();
							result.Result.PrepareDescriptors();

							var gkControllerDevice = result.Result.RootDevice.Children.FirstOrDefault();
							if (gkControllerDevice != null)
							{
								foreach (var zone in result.Result.Zones)
								{
									zone.GkDatabaseParent = gkControllerDevice;
								}
								foreach (var guardZone in result.Result.GuardZones)
								{
									guardZone.GkDatabaseParent = gkControllerDevice;
								}
								foreach (var direction in result.Result.Directions)
								{
									direction.GkDatabaseParent = gkControllerDevice;
								}
								foreach (var pumpStation in result.Result.PumpStations)
								{
									pumpStation.GkDatabaseParent = gkControllerDevice;
								}
								foreach (var mpt in result.Result.MPTs)
								{
									mpt.GkDatabaseParent = gkControllerDevice;
								}
								foreach (var delay in result.Result.Delays)
								{
									delay.GkDatabaseParent = gkControllerDevice;
								}
								foreach (var code in result.Result.Codes)
								{
									code.GkDatabaseParent = gkControllerDevice;
								}
							}

							configurationCompareViewModel = new ConfigurationCompareViewModel(GKManager.DeviceConfiguration, result.Result, SelectedDevice.Device);
						});
						LoadingService.Close();
						if (configurationCompareViewModel.Error != null)
						{
							MessageBoxService.ShowWarning(configurationCompareViewModel.Error, "Ошибка при чтении конфигурации");
							return;
						}
						if (DialogService.ShowModalWindow(configurationCompareViewModel))
							ServiceFactoryBase.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
					}
					else
					{
						LoadingService.Close();
						MessageBoxService.ShowError(result.Error, "Ошибка при чтении конфигурации");
					}
				}));
			});
			thread.Name = "DeviceCommandsViewModel ReadConfiguration";
			thread.Start();
		}

		public RelayCommand ReadConfigFileCommand { get; private set; }
		void OnReadConfigFile()
		{
			_devicesViewModel.DeviceToCompareConfiguration = SelectedDevice.Device;
			var result = ClientManager.FiresecService.GKReadConfigurationFromGKFile(SelectedDevice.Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error, "Ошибка при чтении конфигурации");
			}
		}

		bool CanReadConfiguration()
		{
			return (SelectedDevice != null && (SelectedDevice.Device.Driver.IsKau || SelectedDevice.Driver.DriverType == GKDriverType.GK || SelectedDevice.Device.DriverType == GKDriverType.GKMirror));
		}

		bool CanReadConfigFile()
		{
			return (SelectedDevice != null && SelectedDevice.Driver.DriverType == GKDriverType.GK);
		}

		//public RelayCommand UpdateFirmwareCommand { get; private set; }
		//void OnUpdateFirmware()
		//{
		//	var openDialog = new OpenFileDialog()
		//	{
		//		Filter = "soft update files|*.hcs",
		//		DefaultExt = "soft update files|*.hcs"
		//	};
		//	if (openDialog.ShowDialog().Value)
		//	{
		//		var thread = new Thread(() =>
		//		{
		//			List<byte> firmwareBytes = GKProcessor.FirmwareUpdateHelper.HexFileToBytesList(openDialog.FileName);
		//			var result = ClientManager.FiresecService.GKUpdateFirmware(SelectedDevice.Device, firmwareBytes);

		//			ApplicationService.Invoke(() =>
		//			{
		//				LoadingService.Close();
		//				if (result.HasError)
		//				{
		//					MessageBoxService.ShowWarning(result.Error, "Ошибка при обновление ПО");
		//				}
		//			});
		//		});
		//		thread.Name = "DeviceCommandsViewModel UpdateFirmware";
		//		thread.Start();

		//	}
		//}

		//bool CanUpdateFirmware()
		//{
		//	return (SelectedDevice != null && (SelectedDevice.Driver.IsKau || SelectedDevice.Driver.DriverType == GKDriverType.GK) && ClientManager.CheckPermission(PermissionType.Adm_ChangeDevicesSoft));
		//}

		bool ValidateConfiguration()
		{
			var validationResult = ServiceFactory.ValidationService.Validate();
			if (validationResult.HasErrors(ModuleType.GK))
			{
				if (validationResult.CannotSave(ModuleType.GK) || validationResult.CannotWrite(ModuleType.GK))
				{
					MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
					return false;
				}
			}
			return true;
		}

		public RelayCommand AutoSearchCommand { get; private set; }
		void OnAutoSearch()
		{
			var thread = new Thread(() =>
			{
				var result = ClientManager.FiresecService.GKAutoSearch(SelectedDevice.Device);

				ApplicationService.Invoke(() =>
				{
					if (!result.HasError)
					{
						AutoSearchDevicesViewModel autoSearchDevicesViewModel = null;
						WaitHelper.Execute(() =>
						{
							autoSearchDevicesViewModel = new AutoSearchDevicesViewModel(result.Result, SelectedDevice.Device);
						});
						LoadingService.Close();
						DialogService.ShowModalWindow(autoSearchDevicesViewModel);
					}
					else
					{
						LoadingService.Close();
						MessageBoxService.ShowWarning(result.Error, "Ошибка при автопоиске конфигурации");
					}
				});
			});
			thread.Name = "DeviceCommandsViewModel AutoSearch";
			thread.Start();
		}

		bool CanAutoSearch()
		{
			return (SelectedDevice != null && (SelectedDevice.Driver.DriverType == GKDriverType.GK || SelectedDevice.Driver.DriverType == GKDriverType.RSR2_KAU));
		}

		public RelayCommand GetUsersCommand { get; private set; }
		void OnGetUsers()
		{
			var result = ClientManager.FiresecService.GKGetUsers(SelectedDevice.Device);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error, "Ошибка при получении пользователей");
			}
		}

		bool CanGetUsers()
		{
			return (SelectedDevice != null && SelectedDevice.Driver.DriverType == GKDriverType.GK);
		}

		public RelayCommand RewriteUsersCommand { get; private set; }
		void OnRewriteUsers()
		{
			var result = ClientManager.FiresecService.GKRewriteUsers(SelectedDevice.Device.UID);
			if (result.HasError)
			{
				MessageBoxService.ShowWarning(result.Error, "Ошибка при перезаписи пользователей");
			}
		}

		bool CanRewriteUsers()
		{
			return (SelectedDevice != null && SelectedDevice.Driver.DriverType == GKDriverType.GK);
		}

		public RelayCommand RewriteAllSchedulesCommand { get; private set; }
		void OnRewriteAllSchedules()
		{
			var thread = new Thread(() =>
			{
				var result = ClientManager.FiresecService.GKRewriteAllSchedules(SelectedDevice.Device);

				ApplicationService.Invoke(() =>
				{
					if (!result.HasError)
					{
					}
					else
					{
						LoadingService.Close();
						MessageBoxService.ShowWarning(result.Error, "Ошибка при перезаписи всех графиков");
					}
				});
			});
			thread.Name = "DeviceCommandsViewModel RewriteAllSchedules";
			thread.Start();
		}

		bool CanRemoveAllSchedules()
		{
			return (SelectedDevice != null && SelectedDevice.Driver.DriverType == GKDriverType.GK);
		}

		bool CheckNeedSave(bool syncParameters = false)
		{
			if (ServiceFactory.SaveService.GKChanged)
			{
				if (MessageBoxService.ShowQuestion("Для выполнения этой операции необходимо применить конфигурацию. Применить сейчас?"))
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