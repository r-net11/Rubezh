using System;
using System.Linq;
using Common;
using Firesec;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System.Collections.Generic;

namespace DevicesModule.ViewModels
{
	public class FS2DeviceCommandsAuParametersViewModel
	{
		DevicesViewModel DevicesViewModel;
		public DeviceViewModel SelectedDevice
		{
			get { return DevicesViewModel.SelectedDevice; }
		}

		public FS2DeviceCommandsAuParametersViewModel(DevicesViewModel devicesViewModel)
		{
			ResetConfigurationParametersCommand = new RelayCommand(OnResetConfigurationParameters, CanGetConfigurationParameters);
			GetConfigurationParametersCommand = new RelayCommand(OnGetConfigurationParameters, CanGetConfigurationParameters);
			SetConfigurationParametersCommand = new RelayCommand(OnSetConfigurationParameters, CanSetConfigurationParameters);
			GetAllDeviceConfigurationParametersCommand = new RelayCommand(OnGetAllDeviceConfigurationParameters, CanGetAllDeviceConfigurationParameters);
			SetAllDeviceConfigurationParametersCommand = new RelayCommand(OnSetAllDeviceConfigurationParameters, CanSetAllDeviceConfigurationParameters);
			DevicesViewModel = devicesViewModel;
		}

		public RelayCommand ResetConfigurationParametersCommand { get; private set; }
		void OnResetConfigurationParameters()
		{
			foreach (var property in SelectedDevice.Device.Properties)
			{
				try
				{
					var driverProperty = SelectedDevice.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (driverProperty != null)
					{
						property.Value = driverProperty.Default;
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "DeviceCommandsAuParametersViewModel.OnResetConfigurationParameters");
				}
				SelectedDevice.UpdateConfigurationProperties();
			}
			ServiceFactory.SaveService.FSParametersChanged = true;
		}

		public RelayCommand GetConfigurationParametersCommand { get; private set; }
		void OnGetConfigurationParameters()
		{
			WaitHelper.Execute(() =>
			{
				var result = FiresecManager.FS2ClientContract.GetConfigurationParameters(SelectedDevice.Device.UID);
				if (result.HasError)
				{
					MessageBoxService.Show("При вызове метода на сервере возникло исключение " + result.Error);
					return;
				}
				CopyPropertiesToDevice(SelectedDevice, result.Result);
			});
			ServiceFactory.SaveService.FSParametersChanged = true;
		}
		bool CanGetConfigurationParameters()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.HasConfigurationProperties) &&
				(SelectedDevice.Device.Parent != null) && (!SelectedDevice.Device.Parent.IsMonitoringDisabled));
		}

		public RelayCommand SetConfigurationParametersCommand { get; private set; }
		void OnSetConfigurationParameters()
		{
			WaitHelper.Execute(() =>
			{
				foreach (var property in SelectedDevice.Device.Properties)
				{
					var driverProperty = SelectedDevice.Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (IsPropertyValid(property, driverProperty))
					{
						MessageBoxService.Show("Значение параметра \n" + driverProperty.Caption + "\nдолжно быть целым числом " + "в диапазоне от " + driverProperty.Min.ToString() + " до " + driverProperty.Max.ToString(), "Firesec");
						return;
					}
				}
				FiresecManager.FS2ClientContract.SetConfigurationParameters(SelectedDevice.Device.UID, SelectedDevice.Device.Properties);
			});
		}

		bool IsPropertyValid(Property property, DriverProperty driverProperty)
		{
			int value;
			return
					driverProperty != null &&
					driverProperty.IsAUParameter &&
					driverProperty.DriverPropertyType == DriverPropertyTypeEnum.IntType &&
					(!int.TryParse(property.Value, out value) ||
					(value < driverProperty.Min || value > driverProperty.Max));
		}

		bool CanSetConfigurationParameters()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.HasConfigurationProperties) &&
				(SelectedDevice.Device.Parent != null) && (!SelectedDevice.Device.Parent.IsMonitoringDisabled));
		}

		public RelayCommand SetAllDeviceConfigurationParametersCommand { get; private set; }
		void OnSetAllDeviceConfigurationParameters()
		{
			foreach (var childDevice in SelectedDevice.Children)
			{
				foreach (var property in childDevice.Device.Properties)
				{
					var prop = childDevice.Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (prop != null &&
						prop.DriverPropertyType == DriverPropertyTypeEnum.IntType &&
						(int.Parse(property.Value) < prop.Min || int.Parse(property.Value) > prop.Max)
						)
					{
						property.Value = prop.Min.ToString();
						MessageBoxService.Show("Значение параметра " + prop.Caption + " должно находиться в диапазоне от " + prop.Min.ToString() + " до " + prop.Max.ToString(), "Firesec");
						return;
					}
				}
			}

			try
			{
				LoadingService.ShowProgress("", "Запись параметров в дочерние устройства", SelectedDevice.Children.Count());
				foreach (var childDevice in SelectedDevice.Children)
				{
					LoadingService.DoStep(childDevice.Device.Driver.ShortName + " " + childDevice.Address);
					FiresecManager.FS2ClientContract.SetConfigurationParameters(childDevice.Device.UID, childDevice.Device.Properties);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "DeviceCommandsAuParametersViewModel.OnSetAllDeviceConfigurationParameters");
			}
			finally
			{
				LoadingService.Close();
			}
		}
		bool CanSetAllDeviceConfigurationParameters()
		{
			if (SelectedDevice != null)
			{
				if (SelectedDevice.Device.IsMonitoringDisabled)
					return false;

				foreach (var childDevice in SelectedDevice.Children)
				{
					if ((childDevice != null) && (childDevice.Device.Driver.HasConfigurationProperties))
					{
						return true;
					}
				}
			}
			return false;
		}

		public RelayCommand GetAllDeviceConfigurationParametersCommand { get; private set; }
		void OnGetAllDeviceConfigurationParameters()
		{
			try
			{
				LoadingService.ShowProgress("", "Считывание параметров дочерних устройств", SelectedDevice.Children.Count());
                foreach (var child in SelectedDevice.Children)
				{
					LoadingService.DoStep(child.Device.PresentationAddressAndName);
					var result = FiresecManager.FS2ClientContract.GetConfigurationParameters(child.Device.UID);
					if (result.HasError)
					{
						MessageBoxService.Show("При вызове метода возникло исключение " + result.Error);
						return;
					}
					CopyPropertiesToDevice(child, result.Result);
					ServiceFactory.SaveService.FSParametersChanged = true;

                    foreach (var groupChild in child.Children)
                    {
                        LoadingService.DoStep(groupChild.Device.PresentationAddressAndName);
						var result2 = FiresecManager.FS2ClientContract.GetConfigurationParameters(groupChild.Device.UID);
                        if (result2.HasError)
                        {
                            MessageBoxService.Show("При вызове метода возникло исключение " + result2.Error);
                            return;
                        }
						CopyPropertiesToDevice(groupChild, result.Result);
						ServiceFactory.SaveService.FSParametersChanged = true;
                    }
				};
			}
			catch (Exception e)
			{
				Logger.Error(e, "DeviceCommandsAuParametersViewModel.OnGetAllDeviceConfigurationParameters");
			}
			finally
			{
				LoadingService.Close();
			}
		}
		bool CanGetAllDeviceConfigurationParameters()
		{
			if (SelectedDevice != null)
			{
				if(SelectedDevice.Device.IsMonitoringDisabled)
					return false;

				foreach (var childDevice in SelectedDevice.Children)
				{
					if ((childDevice != null) && (childDevice.Device.Driver.HasConfigurationProperties))
					{
						return true;
					}
				}
			}
			return false;
		}

		void CopyPropertiesToDevice(DeviceViewModel deviceViewModel, List<Property> properties)
		{
			foreach (var deviceProperty in properties)
			{
				var property = SelectedDevice.Device.Properties.FirstOrDefault(x => x.Name == deviceProperty.Name);
				if (property != null)
				{
					property.Value = deviceProperty.Value;
				}
				else
				{
					deviceViewModel.Device.Properties.Add(deviceProperty);
				}
			}
			deviceViewModel.PropertiesViewModel.IsAuParametersReady = false;
		}
	}
}