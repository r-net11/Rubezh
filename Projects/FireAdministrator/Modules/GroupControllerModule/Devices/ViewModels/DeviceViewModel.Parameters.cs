﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.GK;
using FiresecClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	public partial class DeviceViewModel
	{
		void InitializeParamsCommands()
		{
			ReadCommand = new RelayCommand(OnRead, CanReadWrite);
			WriteCommand = new RelayCommand(OnWrite, CanReadWrite);
			ReadAllCommand = new RelayCommand(OnReadAll, CanReadWriteAll);
			WriteAllCommand = new RelayCommand(OnWriteAll, CanReadWriteAll);
			SyncFromDeviceToSystemCommand = new RelayCommand(OnSyncFromDeviceToSystem, CanSync);
			SyncFromAllDeviceToSystemCommand = new RelayCommand(OnSyncFromAllDeviceToSystem, CanSyncAll);

			CopyParamCommand = new RelayCommand(OnCopy, CanCopy);
			PasteParamCommand = new RelayCommand(OnPaste, CanPaste);
			PasteAllParamCommand = new RelayCommand(OnPasteAll, CanPasteAll);
			ResetAUPropertiesCommand = new RelayCommand(OnResetAUProperties);
		}

		public List<GKDevice> GetRealChildren()
		{
			return Device.AllChildrenAndSelf.Where(device => device.Driver.Properties.Any(x => x.IsAUParameter)).ToList();
		}

		#region Capy and Paste
		public static GKDriver DriverCopy;
		public static List<GKProperty> PropertiesCopy;

		public RelayCommand CopyParamCommand { get; private set; }
		void OnCopy()
		{
			DriverCopy = Device.Driver;
			PropertiesCopy = new List<GKProperty>();
			foreach (var property in Device.Properties)
			{
				var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (driverProperty != null && driverProperty.IsAUParameter)
				{
					var propertyCopy = new GKProperty()
					{
						StringValue = property.StringValue,
						Name = property.Name,
						Value = property.Value,
						DriverProperty = driverProperty
					};
					PropertiesCopy.Add(propertyCopy);
				}
			}
		}
		bool CanCopy()
		{
			return HasAUProperties;
		}

		public RelayCommand PasteParamCommand { get; private set; }
		void OnPaste()
		{
			CopyParametersFromBuffer(Device);
			PropertiesViewModel.Update();
		}
		bool CanPaste()
		{
			return DriverCopy != null && Device.DriverType == DriverCopy.DriverType;
		}

		public RelayCommand PasteAllParamCommand { get; private set; }
		void OnPasteAll()
		{
			foreach (var device in Device.AllChildrenAndSelf)
			{
				if (device.DriverType == DriverCopy.DriverType)
				{
					CopyParametersFromBuffer(device);
					var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
					if (deviceViewModel != null)
						deviceViewModel.PropertiesViewModel.Update();
				}
			}
		}
		bool CanPasteAll()
		{
			return (Children.Count() > 0 && DriverCopy != null) && (Device.AllChildrenAndSelf.Any(x => x.DriverType == DriverCopy.DriverType));
		}

		static void CopyParametersFromBuffer(GKDevice device)
		{
			foreach (var property in PropertiesCopy)
			{
				var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (deviceProperty != null)
				{
					deviceProperty.Value = property.Value;
				}
			}
			ServiceFactory.SaveService.GKChanged = true;
		}
		#endregion

		public RelayCommand WriteCommand { get; private set; }
		void OnWrite()
		{
			if (CompareLocalWithRemoteHashes())
			{
				var device = new List<GKDevice> { Device };
				if (Validate(device))
				{
					LoadingService.Show("Запись параметров в устройствo " + Device.PresentationName, null, 1, true);
					if (WriteDevices(device))
					{
						SyncFromSystemToDeviceProperties(device);
					}
				}
			}
		}

		public RelayCommand WriteAllCommand { get; private set; }
		void OnWriteAll()
		{
			if (CompareLocalWithRemoteHashes())
			{
				var devices = GetRealChildren();
				if (Validate(devices))
				{
					LoadingService.Show("Запись параметров в дочерние устройства " + Device.PresentationName, null, 1, true);
					if (WriteDevices(devices))
					{
						SyncFromSystemToDeviceProperties(devices);
					}
				}
			}
		}

		public void SyncFromSystemToDeviceProperties(List<GKDevice> devices)
		{
			foreach (var device in devices)
			{
				foreach (var property in device.Properties)
				{
					if (property.DriverProperty != null && property.DriverProperty.IsAUParameter)
					{
						var deviceProperty = device.DeviceProperties.FirstOrDefault(x => x.Name == property.Name);
						if (deviceProperty == null)
						{
							device.DeviceProperties.Add(new GKProperty
								{
									DriverProperty = property.DriverProperty,
									Name = property.Name,
									Value = property.Value
								});
							ServiceFactory.SaveService.GKChanged = true;
						}
						if (deviceProperty != null && deviceProperty.Value != property.Value)
						{
							deviceProperty.Value = property.Value;
							ServiceFactory.SaveService.GKChanged = true;
						}
					}
				}
				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
				if (deviceViewModel != null)
					deviceViewModel.PropertiesViewModel.Update();
			}
		}

		public RelayCommand SyncFromDeviceToSystemCommand { get; private set; }
		void OnSyncFromDeviceToSystem()
		{
			CheckShleif();
			CopyFromDeviceToSystem(Device);
			PropertiesViewModel.Update();
		}

		public RelayCommand SyncFromAllDeviceToSystemCommand { get; private set; }
		void OnSyncFromAllDeviceToSystem()
		{
			var devices = GetRealChildren();
			foreach (var device in devices)
			{
				CopyFromDeviceToSystem(device);
				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
				if (deviceViewModel != null)
					deviceViewModel.PropertiesViewModel.Update();
			}
		}

		bool CanSync()
		{
			return HasAUProperties;
		}

		bool CanSyncAll()
		{
			return Children.Count() > 0;
		}

		static void CopyFromDeviceToSystem(GKDevice device)
		{
			foreach (var deviceProperty in device.DeviceProperties)
			{
				if (deviceProperty.DriverProperty.IsAUParameter)
				{
					var property = device.Properties.FirstOrDefault(x => x.Name == deviceProperty.Name);
					if (property != null)
					{
						property.Value = deviceProperty.Value;
					}
				}
			}
			ServiceFactory.SaveService.GKChanged = true;
		}

		bool Validate(IEnumerable<GKDevice> devices)
		{
			foreach (var device in devices)
			{
				foreach (var property in device.Properties)
				{
					var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (IsPropertyValid(property, driverProperty))
					{
						string message;
						if (driverProperty.Multiplier != 0)
						{
							message = "Устройство " + device.PresentationName +
								"\nЗначение параметра\n" +
								driverProperty.Caption +
								"\nдолжно быть в диапазоне от " + (driverProperty.Min / driverProperty.Multiplier).ToString() +
								" до " + (driverProperty.Max / driverProperty.Multiplier).ToString();
						}
						else
						{
							message = "Устройство " + device.PresentationName +
								"\nЗначение параметра\n" +
								driverProperty.Caption +
								"\nдолжно быть целым числом в диапазоне от " + driverProperty.Min.ToString() +
								" до " + driverProperty.Max.ToString();
						}
						MessageBoxService.Show(message);
						return false;
					}
				}
			}
			return true;
		}

		static bool IsPropertyValid(GKProperty property, GKDriverProperty driverProperty)
		{
			int value;
			return
					driverProperty != null &&
					driverProperty.IsAUParameter &&
					driverProperty.DriverPropertyType == GKDriverPropertyTypeEnum.IntType &&
					(!int.TryParse(Convert.ToString(property.Value), out value) ||
					(value < driverProperty.Min || value > driverProperty.Max));
		}

		public RelayCommand ReadCommand { get; private set; }
		void OnRead()
		{
			if (CompareLocalWithRemoteHashes())
			{
				LoadingService.Show("Чтение параметров из устройства " + Device.PresentationName, null, 1, true);
				ReadDevices(new List<GKDevice> { Device });
				PropertiesViewModel.Update();
			}
		}

		bool CanReadWrite()
		{
			return HasAUProperties;
		}

		public RelayCommand ReadAllCommand { get; private set; }
		void OnReadAll()
		{
			if (CompareLocalWithRemoteHashes())
			{
				var devices = GetRealChildren();
				LoadingService.Show("Чтение параметров из дочерних устройств " + Device.PresentationName, null, 1, true);
				ReadDevices(devices);
			}
		}

		bool CanReadWriteAll()
		{
			return Children.Count() > 0 && Device.DriverType != GKDriverType.System;
		}

		public bool HasAUProperties
		{
			get { return Device.Driver.Properties.Count(x => x.IsAUParameter) > 0; }
		}

		static void ReadDevices(IEnumerable<GKDevice> devices)
		{
			var errorLog = "";
			DescriptorsManager.Create();
			var i = 0;
			LoadingService.AddCount(devices.Count());
			foreach (var device in devices)
			{
				if (LoadingService.IsCanceled)
					break;
				i++;
				LoadingService.DoStep("Запрос параметров объекта " + i);
				var result = FiresecManager.FiresecService.GKGetSingleParameter(device);
				if (!result.HasError)
				{
					foreach (var property in result.Result)
					{
						var deviceProperty = device.DeviceProperties.FirstOrDefault(x => x.Name == property.Name);
						if (deviceProperty == null)
						{
							deviceProperty = new GKProperty()
							{
								Name = property.Name,
								DriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name)
							};
							device.DeviceProperties.Add(deviceProperty);
						}
						deviceProperty.Value = property.Value;
						deviceProperty.StringValue = property.StringValue;
					}
				}
				else
				{
					errorLog += "\n" + device.PresentationName;
				}
			}
			LoadingService.Close();
			if (errorLog != "")
				MessageBoxService.ShowError("Ошибка при получении параметров следующих устройств:" + errorLog);
		}

		static bool WriteDevices(IEnumerable<GKDevice> devices)
		{
			var errorLog = "";
			DescriptorsManager.Create();
			var i = 0;
			LoadingService.AddCount(devices.Count());
			foreach (var device in devices)
			{
				if (LoadingService.IsCanceled)
					break;
				i++;
				LoadingService.DoStep("Запись параметров объекта " + i);
				var baseDescriptor = ParametersHelper.GetBaseDescriptor(device);
				if (baseDescriptor != null)
				{
					var result = FiresecManager.FiresecService.GKSetSingleParameter(device, baseDescriptor.Parameters);
					if (result.HasError)
						errorLog += "\n" + device.PresentationName + ". " + result.Error;
				}
				else
				{
					errorLog += "\n" + device.PresentationName + ". Не найден дескриптор";
				}
				if (devices.Count() > 1 && i < devices.Count())
					Thread.Sleep(100);
			}
			LoadingService.Close();
			if (errorLog != "")
			{
				MessageBoxService.ShowWarning("Ошибка при записи параметров в следующие устройства:" + errorLog);
				return false;
			}
			return true;
		}

		public RelayCommand ResetAUPropertiesCommand { get; private set; }
		void OnResetAUProperties()
		{
			foreach (var property in Device.Properties)
			{
				var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (driverProperty != null)
				{
					property.Value = driverProperty.Default;
				}
			}
			PropertiesViewModel = new PropertiesViewModel(Device);
			OnPropertyChanged(() => PropertiesViewModel);
		}

		bool CompareLocalWithRemoteHashes()
		{
			var gkParent = DevicesViewModel.Current.SelectedDevice.Device;
			if (gkParent.DriverType != GKDriverType.GK)
				gkParent = DevicesViewModel.Current.SelectedDevice.Device.GKParent;

			var result = FiresecManager.FiresecService.GKGKHash(gkParent);
			if (result.HasError)
			{
				MessageBoxService.ShowError(result.Error + ". Операция запрещена");
				return false;
			}

			var localHash = GKFileInfo.CreateHash1(GKManager.DeviceConfiguration, gkParent);
			var remoteHash = result.Result;
			if (GKFileInfo.CompareHashes(localHash, remoteHash))
				return true;
			MessageBoxService.ShowError("Конфигурации различны. Операция запрещена");
			return false;
		}
	}
}