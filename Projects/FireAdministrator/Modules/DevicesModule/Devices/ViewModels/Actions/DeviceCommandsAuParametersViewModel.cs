using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI;
using Infrastructure.Common.Windows;
using FiresecClient;
using FiresecAPI.Models;
using Firesec;
using Common;

namespace DevicesModule.ViewModels
{
    public class DeviceCommandsAuParametersViewModel
    {
        DevicesViewModel DevicesViewModel;
        public DeviceViewModel SelectedDevice
        {
            get { return DevicesViewModel.SelectedDevice; }
        }

        public DeviceCommandsAuParametersViewModel(DevicesViewModel devicesViewModel)
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
                SelectedDevice.UpdataConfigurationProperties();
            }
        }

        public RelayCommand GetConfigurationParametersCommand { get; private set; }
        void OnGetConfigurationParameters()
        {
            WaitHelper.Execute(() =>
            {
                OperationResult<bool> result = FiresecDriverAuParametersHelper.BeginGetConfigurationParameters(SelectedDevice.Device);
                if (result.HasError)
                {
                    MessageBoxService.Show("При вызове метода на сервере возникло исключение " + result.Error);
                    return;
                }
                SelectedDevice.PropertiesViewModel.IsAuParametersReady = false;
            });
        }
        bool CanGetConfigurationParameters()
        {
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.HasConfigurationProperties));
        }

        public RelayCommand SetConfigurationParametersCommand { get; private set; }
        void OnSetConfigurationParameters()
        {
            WaitHelper.Execute(() =>
            {
                foreach (var property in SelectedDevice.Device.Properties)
                {
                    var driverProperty = SelectedDevice.Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
                    if (ValidSet(property, driverProperty))
                    {
                        MessageBoxService.Show("Значение параметра \n" + driverProperty.Caption + "\nдолжно быть целым числом " + "в диапазоне от " + driverProperty.Min.ToString() + " до " + driverProperty.Max.ToString(), "Firesec");
                        return;
                    }
                }
                FiresecDriverAuParametersHelper.SetConfigurationParameters(SelectedDevice.Device.UID, SelectedDevice.Device.Properties);
            });
        }

        bool ValidSet(Property property, DriverProperty driverProperty)
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
            return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.HasConfigurationProperties));
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
                    FiresecDriverAuParametersHelper.SetConfigurationParameters(childDevice.Device.UID, childDevice.Device.Properties);
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
            foreach (var childDevice in SelectedDevice.Children)
            {
                if ((childDevice != null) && (childDevice.Device.Driver.HasConfigurationProperties))
                {
                    return true;
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
                    var result = FiresecDriverAuParametersHelper.BeginGetConfigurationParameters(child.Device);
                    if (result.HasError)
                    {
                        MessageBoxService.Show("При вызове метода возникло исключение " + result.Error);
                        return;
                    }
                    child.PropertiesViewModel.IsAuParametersReady = false;
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
            foreach (var childDevice in SelectedDevice.Children)
            {
                if ((childDevice != null) && (childDevice.Device.Driver.HasConfigurationProperties))
                {
                    return true;
                }
            }
            return false;
        }
    }
}