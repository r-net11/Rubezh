using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.ViewModels;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Microsoft.Win32;

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
			return (SelectedDevice != null && (SelectedDevice.Device.Driver.IsKauOrRSR2Kau || SelectedDevice.Device.DriverType == GKDriverType.GK));
		}

		public RelayCommand SynchroniseTimeCommand { get; private set; }
		void OnSynchroniseTime()
		{
			var result = FiresecManager.FiresecService.GKSyncronyseTime(SelectedDevice.Device);
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
			return (SelectedDevice != null && SelectedDevice.Device.DriverType == GKDriverType.GK);
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
					var thread = new Thread(() =>
					{
						var result = FiresecManager.FiresecService.GKWriteConfiguration(SelectedDevice.Device.DriverType == GKDriverType.GK ? SelectedDevice.Device : SelectedDevice.Device.GKParent);
						ApplicationService.Invoke(() =>
						{
							LoadingService.Close();
							if (result.HasError)
							{
								MessageBoxService.ShowWarning(result.Error);
							}
						});
					}) { Name = "DeviceCommandsViewModel WriteConfig" };
					thread.Start();
				}
			}
		}

		bool CanWriteConfig()
		{
			return FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig) &&
				SelectedDevice != null &&
				SelectedDevice.Device.DriverType != GKDriverType.System;
		}

		public RelayCommand ReadConfigurationCommand { get; private set; }
		void OnReadConfiguration()
		{
			var thread = new Thread(() =>
			{
				DescriptorsManager.Create();
				var result = FiresecManager.FiresecService.GKReadConfiguration(SelectedDevice.Device);

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

							configurationCompareViewModel = new ConfigurationCompareViewModel(GKManager.DeviceConfiguration, result.Result, SelectedDevice.Device, false);
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
			var thread = new Thread(() =>
			{
				var result = FiresecManager.FiresecService.GKReadConfigurationFromGKFile(SelectedDevice.Device);

				ApplicationService.Invoke(new Action(() =>
				{
					if (!result.HasError)
					{
						ConfigurationCompareViewModel configurationCompareViewModel = null;
						WaitHelper.Execute(() =>
						{
							DescriptorsManager.Create();
							result.Result.UpdateConfiguration();
							result.Result.PrepareDescriptors();
							configurationCompareViewModel = new ConfigurationCompareViewModel(GKManager.DeviceConfiguration, result.Result, SelectedDevice.Device, true);
						});
						LoadingService.Close();
						if (configurationCompareViewModel.Error != null)
						{
							MessageBoxService.ShowError(configurationCompareViewModel.Error, "Ошибка при чтении конфигурации");
							return;
						}
						if (DialogService.ShowModalWindow(configurationCompareViewModel))
							ServiceFactoryBase.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
					}
					else
					{
						LoadingService.Close();
						MessageBoxService.ShowWarning(result.Error, "Ошибка при чтении конфигурационного файла");
					}
				}));
			});
			thread.Name = "DeviceCommandsViewModel ReadConfigFile";
			thread.Start();
		}

		bool CanReadConfiguration()
		{
			return (SelectedDevice != null && (SelectedDevice.Device.Driver.IsKauOrRSR2Kau || SelectedDevice.Driver.DriverType == GKDriverType.GK));
		}

		bool CanReadConfigFile()
		{
			return (SelectedDevice != null && SelectedDevice.Driver.DriverType == GKDriverType.GK);
		}

		public RelayCommand UpdateFirmwhareCommand { get; private set; }
		void OnUpdateFirmwhare()
		{
			if (SelectedDevice.Device.DriverType == GKDriverType.System)
			{
				var openDialog = new OpenFileDialog()
				{
					Filter = "FSCS updater|*.fscs",
					DefaultExt = "FSCS updater|*.fscs"
				};
				if (openDialog.ShowDialog().Value)
				{
					var gkKauKauRsr2Devices = GKManager.DeviceConfiguration.Devices.FindAll(x => (x.Driver.DriverType == GKDriverType.GK) || (x.Driver.IsKauOrRSR2Kau));
					var firmWareUpdateViewModel = new FirmWareUpdateViewModel(gkKauKauRsr2Devices);
					if (DialogService.ShowModalWindow(firmWareUpdateViewModel))
					{
						var thread = new Thread(() =>
						{
							var hxcFileInfo = HXCFileInfoHelper.Load(openDialog.FileName);
							var devices = new List<GKDevice>();
							firmWareUpdateViewModel.UpdatedDevices.FindAll(x => x.IsChecked).ForEach(x => devices.Add(x.Device));
							var result = FiresecManager.FiresecService.GKUpdateFirmwareFSCS(hxcFileInfo, devices);

							ApplicationService.Invoke(new Action(() =>
							{
								LoadingService.Close();
								if (result.HasError)
								{
									MessageBoxService.ShowWarning(result.Error, "Ошибка при обновление ПО");
								}
							}));
						});
						thread.Name = "DeviceCommandsViewModel UpdateFirmwhare";
						thread.Start();
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
				if (openDialog.ShowDialog().Value)
				{
					var thread = new Thread(() =>
					{
						var result = FiresecManager.FiresecService.GKUpdateFirmware(SelectedDevice.Device, openDialog.FileName);

						ApplicationService.Invoke(new Action(() =>
						{
							LoadingService.Close();
							if (result.HasError)
							{
								MessageBoxService.ShowWarning(result.Error, "Ошибка при обновление ПО");
							}
						}));
					});
					thread.Name = "DeviceCommandsViewModel UpdateFirmwhare";
					thread.Start();
				}

			}
		}

		bool CanUpdateFirmwhare()
		{
			return (SelectedDevice != null && (SelectedDevice.Driver.IsKauOrRSR2Kau || SelectedDevice.Driver.DriverType == GKDriverType.GK || SelectedDevice.Driver.DriverType == GKDriverType.System) && FiresecManager.CheckPermission(PermissionType.Adm_ChangeDevicesSoft));
		}

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