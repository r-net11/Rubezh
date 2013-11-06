using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.XModels;
using FiresecClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public partial class DeviceViewModel
    {
		List<XDevice> GetRealChildren()
		{
			var devices = XManager.GetAllDeviceChildren(Device).Where(device => device.Driver.Properties.Any(x => x.IsAUParameter)).ToList();
			return devices;
		}

        #region Capy and Paste
        public static XDriver DriverCopy;
        public static List<XProperty> PropertiesCopy;

        public RelayCommand CopyParamCommand { get; private set; }
        void OnCopy()
        {
            DriverCopy = Device.Driver;
            PropertiesCopy = new List<XProperty>();
            foreach (var property in Device.Properties)
            {
                var driverProperty = Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
                if (driverProperty != null && driverProperty.IsAUParameter)
                {
                    var propertyCopy = new XProperty()
                    {
                        StringValue = property.StringValue,
                        Name = property.Name,
                        Value = property.Value
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
            //UpdateDeviceParameterMissmatch();
        }
        bool CanPaste()
        {
            return DriverCopy != null && Device.DriverType == DriverCopy.DriverType;
        }

        public RelayCommand PasteAllParamCommand { get; private set; }
        void OnPasteAll()
        {
            foreach (var device in XManager.GetAllDeviceChildren(Device))
            {
                CopyParametersFromBuffer(device);
                var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
                if (deviceViewModel != null)
                    deviceViewModel.PropertiesViewModel.Update();
            }
            //UpdateDeviceParameterMissmatch();
        }
        bool CanPasteAll()
        {
            return Children.Count() > 0 && DriverCopy != null;
        }

        static void CopyParametersFromBuffer(XDevice device)
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

        #region Template
        public RelayCommand PasteTemplateCommand { get; private set; }
        void OnPasteTemplate()
        {
            var parameterTemplateSelectationViewModel = new ParameterTemplateSelectationViewModel();
            if (DialogService.ShowModalWindow(parameterTemplateSelectationViewModel))
            {
                CopyParametersFromTemplate(parameterTemplateSelectationViewModel.SelectedParameterTemplate, Device);
                PropertiesViewModel.Update();
            }
            //UpdateDeviceParameterMissmatch();
        }
        bool CanPasteTemplate()
        {
            return HasAUProperties;
        }

        public RelayCommand PasteAllTemplateCommand { get; private set; }
        void OnPasteAllTemplate()
        {
            var parameterTemplateSelectationViewModel = new ParameterTemplateSelectationViewModel();
            if (DialogService.ShowModalWindow(parameterTemplateSelectationViewModel))
            {
                var devices = XManager.GetAllDeviceChildren(Device);
                devices.Add(Device);
                foreach (var device in devices)
                {
                    CopyParametersFromTemplate(parameterTemplateSelectationViewModel.SelectedParameterTemplate, device);
                    var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
                    if (deviceViewModel != null)
                        deviceViewModel.PropertiesViewModel.Update();
                }
            }
            //UpdateDeviceParameterMissmatch();
        }
        bool CanPasteAllTemplate()
        {
            return Children.Count() > 0;
        }

        static void CopyParametersFromTemplate(XParameterTemplate parameterTemplate, XDevice device)
        {
            var deviceParameterTemplate = parameterTemplate.DeviceParameterTemplates.FirstOrDefault(x => x.XDevice.DriverUID == device.Driver.UID);
            if (deviceParameterTemplate != null)
            {
                foreach (var property in deviceParameterTemplate.XDevice.Properties)
                {
                    var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == property.Name);
                    if (deviceProperty != null)
                    {
                        deviceProperty.Value = property.Value;
                    }
                }
            }
        }
        #endregion

        public RelayCommand WriteCommand { get; private set; }
        void OnWriteCommand()
        {
            if (CheckNeedSave())
            {
                var devices = new List<XDevice> { Device };
                if (Validate(devices))
                {
					WriteDevices(devices);
                    CopyFromSystemToDevice(Device);
                    PropertiesViewModel.Update();
                }
            }
        }

		void InitializeParamsCommands()
		{
			ReadCommand = new RelayCommand(OnRead, CanReadWrite);
			WriteCommand = new RelayCommand(OnWriteCommand, CanSync);
			ReadAllCommand = new RelayCommand(OnReadAll, CanReadWriteAll);
			WriteAllCommand = new RelayCommand(OnWriteAll, CanSyncAll);
			SyncFromDeviceToSystemCommand = new RelayCommand(OnSyncFromDeviceToSystem, CanSync);
			SyncFromAllDeviceToSystemCommand = new RelayCommand(OnSyncFromAllDeviceToSystem, CanSyncAll);

			CopyParamCommand = new RelayCommand(OnCopy, CanCopy);
			PasteParamCommand = new RelayCommand(OnPaste, CanPaste);
			PasteAllParamCommand = new RelayCommand(OnPasteAll, CanPasteAll);
			PasteTemplateCommand = new RelayCommand(OnPasteTemplate, CanPasteTemplate);
			PasteAllTemplateCommand = new RelayCommand(OnPasteAllTemplate, CanPasteAllTemplate);
			ResetAUPropertiesCommand = new RelayCommand(OnResetAUProperties);
		}
        public RelayCommand WriteAllCommand { get; private set; }
        void OnWriteAll()
        {
            if (CheckNeedSave())
            {
                var devices = GetRealChildren();
                devices.Add(Device);
                if (Validate(devices))
                {
					WriteDevices(devices);
                    foreach (var device in devices)
                    {
                        CopyFromSystemToDevice(device);
                        var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
                        if (deviceViewModel != null)
                            deviceViewModel.PropertiesViewModel.Update();
                    }
                }
            }
        }

		public void SyncFromAllSystemToDevice()
		{
			if (CheckNeedSave())
			{
				var devices = GetRealChildren();
				devices.Add(Device);
				if (Validate(devices))
				{
					foreach (var device in devices)
					{
						CopyFromSystemToDevice(device);
						var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
						if (deviceViewModel != null)
							deviceViewModel.PropertiesViewModel.Update();
					}
				}
			}
		}

        public RelayCommand SyncFromDeviceToSystemCommand { get; private set; }
        void OnSyncFromDeviceToSystem()
        {
            if (CheckNeedSave())
            {
                CopyFromDeviceToSystem(Device);
                PropertiesViewModel.Update();
                //UpdateDeviceParameterMissmatch();
            }
        }

        public RelayCommand SyncFromAllDeviceToSystemCommand { get; private set; }
        void OnSyncFromAllDeviceToSystem()
        {
            if (CheckNeedSave())
            {
                var devices = GetRealChildren();
                devices.Add(Device);
                foreach (var device in devices)
                {
                    CopyFromDeviceToSystem(device);
                    var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
                    if (deviceViewModel != null)
                        deviceViewModel.PropertiesViewModel.Update();
                }
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

        static void CopyFromDeviceToSystem(XDevice device)
        {
            foreach (var deviceProperty in device.DeviceProperties)
            {
            	var property = device.Properties.FirstOrDefault(x => x.Name == deviceProperty.Name);
				property.Value = deviceProperty.Value;
            }
            ServiceFactory.SaveService.GKChanged = true;
        }

        static void CopyFromSystemToDevice(XDevice device)
        {
            foreach (var property in device.Properties)
            {
				if (!property.DriverProperty.IsAUParameter)
					continue;
				var deviceProperty = device.DeviceProperties.FirstOrDefault(x => x.Name == property.Name);
				if (deviceProperty == null)
					device.DeviceProperties.Add(new XProperty
						{
							DriverProperty = property.DriverProperty,
							Name = property.Name,
							Value = property.Value
						});
				else
					deviceProperty.Value = property.Value;
			}
            ServiceFactory.SaveService.GKChanged = true;
        }

        bool Validate(IEnumerable<XDevice> devices)
        {
            foreach (var device in devices)
            {
                foreach (var property in device.Properties)
                {
                    var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
                    if (IsPropertyValid(property, driverProperty))
                    {
                        MessageBoxService.Show("Устройство " + device.PresentationDriverAndAddress + "\nЗначение параметра\n" + driverProperty.Caption + "\nдолжно быть целым числом " + "в диапазоне от " + driverProperty.Min.ToString() + " до " + driverProperty.Max.ToString(), "Firesec");
                        return false;
                    }
                }
            }
            return true;
        }

        static bool IsPropertyValid(XProperty property, XDriverProperty driverProperty)
        {
            int value;
            return
                    driverProperty != null &&
                    driverProperty.IsAUParameter &&
                    driverProperty.DriverPropertyType == XDriverPropertyTypeEnum.IntType &&
                    (!int.TryParse(Convert.ToString(property.Value), out value) ||
                    (value < driverProperty.Min || value > driverProperty.Max));
        }
		
        public RelayCommand ReadCommand { get; private set; }
        void OnRead()
        {
            if (CheckNeedSave())
            {
                ReadDevices(new List<XDevice> { Device });
                PropertiesViewModel.Update();
                ServiceFactory.SaveService.GKChanged = true;
            }
        }

        bool CanReadWrite()
        {
            return HasAUProperties;
        }

        public RelayCommand ReadAllCommand { get; private set; }
        void OnReadAll()
        {
            if (CheckNeedSave())
            {
                var devices = GetRealChildren();
                devices.Add(Device);
                ReadDevices(devices);
            }
        }

        bool CanReadWriteAll()
        {
            return Children.Count() > 0;
        }

        public bool HasAUProperties
        {
            get { return Device.Driver.Properties.Count(x => x.IsAUParameter) > 0; }
        }

        static void ReadDevices(IEnumerable<XDevice> devices)
        {
            ParametersHelper.ErrorLog = "";
            LoadingService.Show("Запрос параметров");
            DatabaseManager.Convert();
            var i = 0;
            LoadingService.AddCount(devices.Count());
            foreach (var device in devices)
            {
				if (LoadingService.IsCanceled)
					break;
                i++;
                ParametersHelper.GetSingleParameter(device);
            }
            LoadingService.Close();
            if (ParametersHelper.ErrorLog != "")
                MessageBoxService.ShowError("Ошибка при получении параметров следующих устройств:" + ParametersHelper.ErrorLog);
            ServiceFactory.SaveService.GKChanged = true;
        }

        static void WriteDevices(IEnumerable<XDevice> devices)
        {
            ParametersHelper.ErrorLog = "";
            LoadingService.Show("Запись параметров");
            DatabaseManager.Convert();
            var i = 0;
            LoadingService.AddCount(devices.Count());
            foreach (var device in devices)
            {
				if (LoadingService.IsCanceled)
					break;
                i++;
                ParametersHelper.SetSingleParameter(device);
				if ((devices.Count() > 1)&&(i < devices.Count()))
					Thread.Sleep(100);
            }
            LoadingService.Close();
            if (ParametersHelper.ErrorLog != "")
                MessageBoxService.ShowError("Ошибка при записи параметров в следующие устройства:" + ParametersHelper.ErrorLog);
            ServiceFactory.SaveService.GKChanged = true;
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
            OnPropertyChanged("PropertiesViewModel");
        }
    }
}
