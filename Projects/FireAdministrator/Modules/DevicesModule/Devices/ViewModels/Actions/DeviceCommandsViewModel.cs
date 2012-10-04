using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		DevicesViewModel _devicesViewModel;

		public DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
		{
			AutoDetectCommand = new RelayCommand(OnAutoDetect, CanAutoDetect);
			ReadDeviceCommand = new RelayCommand<bool>(OnReadDevice, CanReadDevice);
			WriteDeviceCommand = new RelayCommand<bool>(OnWriteDevice, CanWriteDevice);
			WriteAllDeviceCommand = new RelayCommand(OnWriteAllDevice);
			SynchronizeDeviceCommand = new RelayCommand<bool>(OnSynchronizeDevice, CanSynchronizeDevice);
			UpdateSoftCommand = new RelayCommand<bool>(OnUpdateSoft, CanUpdateSoft);
			GetDescriptionCommand = new RelayCommand<bool>(OnGetDescription, CanGetDescription);
			GetDeveceJournalCommand = new RelayCommand<bool>(OnGetDeveceJournal, CanGetDeveceJournal);
			SetPasswordCommand = new RelayCommand<bool>(OnSetPassword, CanSetPassword);
			BindMsCommand = new RelayCommand(OnBindMs, CanBindMs);
			ExecuteCustomAdminFunctionsCommand = new RelayCommand<bool>(OnExecuteCustomAdminFunctions, CanExecuteCustomAdminFunctions);
			GetConfigurationParametersCommand = new RelayCommand(OnGetConfigurationParameters, CanGetConfigurationParameters);
			SetConfigurationParametersCommand = new RelayCommand(OnSetConfigurationParameters, CanSetConfigurationParameters);

            ResetConfigurationParametersCommand = new RelayCommand(OnResetConfigurationParameters, CanGetConfigurationParameters);
			GetAllDeviceConfigurationParametersCommand = new RelayCommand(OnGetAllDeviceConfigurationParameters, CanGetAllDeviceConfigurationParameters);
			SetAllDeviceConfigurationParametersCommand = new RelayCommand(OnSetAllDeviceConfigurationParameters, CanSetAllDeviceConfigurationParameters);
			

			_devicesViewModel = devicesViewModel;
		}

		public DeviceViewModel SelectedDevice
		{
			get { return _devicesViewModel.SelectedDevice; }
		}

		public RelayCommand AutoDetectCommand { get; private set; }
		void OnAutoDetect()
		{
			AutoDetectDeviceHelper.Run(SelectedDevice);
		}

		bool CanAutoDetect()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanAutoDetect));
		}

		#region ReadWriteDevice
		public RelayCommand<bool> ReadDeviceCommand { get; private set; }
		void OnReadDevice(bool isUsb)
		{
			DeviceReadConfigurationHelper.Run(SelectedDevice.Device, isUsb);
		}

		bool CanReadDevice(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanReadDatabase));
		}

		bool ValidateConfiguration()
		{
			var validationResult = ServiceFactory.ValidationService.Validate();
			if (validationResult.CannotSave("FS") || validationResult.CannotWrite("FS"))
			{
				MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
				return false;
			}
			if (validationResult.HasErrors("FS"))
			{
				if (MessageBoxService.ShowQuestion("Конфигурация содержит ошибки. Продолжить") != MessageBoxResult.Yes)
					return false;
			}
			return true;
		}

		public RelayCommand<bool> WriteDeviceCommand { get; private set; }
		void OnWriteDevice(bool isUsb)
		{
			if (ValidateConfiguration())
				DeviceWriteConfigurationHelper.Run(SelectedDevice.Device, isUsb);
		}

		bool CanWriteDevice(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanWriteDatabase));
		}

		public RelayCommand WriteAllDeviceCommand { get; private set; }
		void OnWriteAllDevice()
		{
			if (ValidateConfiguration())
			{
				WriteAllDeviceConfigurationHelper.Run();
			}
		}
		#endregion
		

		public RelayCommand<bool> SynchronizeDeviceCommand { get; private set; }
		void OnSynchronizeDevice(bool isUsb)
		{
			SynchronizeDeviceHelper.Run(SelectedDevice.Device, isUsb);
		}
		bool CanSynchronizeDevice(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanSynchonize));
		}
		bool CanRebootDevice()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanReboot));
		}

		public RelayCommand<bool> UpdateSoftCommand { get; private set; }
		void OnUpdateSoft(bool isUsb)
		{
			FirmwareUpdateHelper.Run(SelectedDevice.Device, isUsb);
		}
		bool CanUpdateSoft(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanUpdateSoft));
		}

		public RelayCommand<bool> GetDescriptionCommand { get; private set; }
		void OnGetDescription(bool isUsb)
		{
			DeviceGetInformationHelper.Run(SelectedDevice.Device, isUsb);
		}
		bool CanGetDescription(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanGetDescription));
		}

		public RelayCommand<bool> GetDeveceJournalCommand { get; private set; }
		void OnGetDeveceJournal(bool isUsb)
		{
			ReadDeviceJournalHelper.Run(SelectedDevice.Device, isUsb);
		}
		bool CanGetDeveceJournal(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanReadJournal));
		}

		public RelayCommand<bool> SetPasswordCommand { get; private set; }
		void OnSetPassword(bool isUsb)
		{
			DialogService.ShowModalWindow(new SetPasswordViewModel(SelectedDevice.Device, isUsb));
		}
		bool CanSetPassword(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanSetPassword));
		}

		public RelayCommand BindMsCommand { get; private set; }
		void OnBindMs()
		{
			DeviceGetSerialListHelper.Run(SelectedDevice.Device);
		}
		bool CanBindMs()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.DriverType == DriverType.MS_1 || SelectedDevice.Device.Driver.DriverType == DriverType.MS_2));
		}

		public RelayCommand<bool> ExecuteCustomAdminFunctionsCommand { get; private set; }
		void OnExecuteCustomAdminFunctions(bool isUsb)
		{
			DialogService.ShowModalWindow(new CustomAdminFunctionsCommandViewModel(SelectedDevice.Device));
		}
		bool CanExecuteCustomAdminFunctions(bool isUsb)
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.CanExecuteCustomAdminFunctions));
		}

        public RelayCommand ResetConfigurationParametersCommand { get; private set; }
        void OnResetConfigurationParameters()
        {
            foreach (var property in SelectedDevice.Device.Properties)
            {
                try
                {
                    string value1 = property.Value;
                    string value2 = SelectedDevice.Driver.Properties.FirstOrDefault(x => x.Name == property.Name).Default;
                    property.Value = value2;
                }
                catch (Exception)
                {
                }
                SelectedDevice.UpdataConfigurationProperties();
            }
        }

		#region GetConfig
		public RelayCommand GetConfigurationParametersCommand { get; private set; }
		void OnGetConfigurationParameters()
		{
			WaitHelper.Execute(() => {
				OperationResult<List<Property>> result = FiresecManager.FiresecDriver.GetConfigurationParameters(SelectedDevice.Device.UID);
				if (!result.HasError)
				{
					foreach (var resultProperty in result.Result)
					{
						var property = SelectedDevice.Device.Properties.FirstOrDefault(x => x.Name == resultProperty.Name);
						if (property == null)
						{
							property = new Property()
							{
								Name = resultProperty.Name
							};
							SelectedDevice.Device.Properties.Add(property);
						}
						property.Value = resultProperty.Value;
					}
					SelectedDevice.UpdataConfigurationProperties();
				}
				else
				{
					MessageBoxService.Show("При вызове метода на сервере возникло исключение " + result.Error);
				}
			});
			
		}

		bool CanGetConfigurationParameters()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.HasConfigurationProperties));
		}
		#endregion

		#region SetConfig
		public RelayCommand SetConfigurationParametersCommand { get; private set; }
		void OnSetConfigurationParameters()
		{
			WaitHelper.Execute(() =>
			{
				foreach (var property in SelectedDevice.Device.Properties)
				{
					var prop = SelectedDevice.Device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
					if (prop != null &&
						prop.DriverPropertyType == DriverPropertyTypeEnum.IntType &&
						(int.Parse(property.Value) < prop.Min || int.Parse(property.Value) > prop.Max)
						)
					{
						MessageBoxService.Show("Значение параметра " + prop.Caption + " должно находиться в диапазоне от " + prop.Min.ToString() + " до " + prop.Max.ToString(), "Firesec");
						return;
					}

				}
				FiresecManager.FiresecDriver.SetConfigurationParameters(SelectedDevice.Device.UID, SelectedDevice.Device.Properties);
			});
		}

		bool CanSetConfigurationParameters()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Device.Driver.HasConfigurationProperties));
		}
		#endregion
		
		#region SetAllCommand 
		public RelayCommand SetAllDeviceConfigurationParametersCommand { get; private set; }
		void OnSetAllDeviceConfigurationParameters()
		{
			LoadingService.ShowProgress("", "Запись параметров в дочерние устройства", SelectedDevice.Children.Count());
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
			foreach (var childDevice in SelectedDevice.Children)
			{
				LoadingService.DoStep(childDevice.Device.Driver.ShortName + " " + childDevice.Address);
				FiresecManager.FiresecDriver.SetConfigurationParameters(childDevice.Device.UID,	childDevice.Device.Properties);
			}
			LoadingService.Close();
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
		#endregion

		#region GetAllCommand
		public RelayCommand GetAllDeviceConfigurationParametersCommand { get; private set; }
		void OnGetAllDeviceConfigurationParameters()
		{
			LoadingService.ShowProgress("", "Считывание параметров дочерних устройств", SelectedDevice.Children.Count());
			
			OperationResult<List<Property>> res;
			foreach (var child in SelectedDevice.Children)
			{
				LoadingService.DoStep(child.Device.Driver.ShortName + " " + child.Address);
				res = FiresecManager.FiresecDriver.GetConfigurationParameters(child.Device.UID);
				if (!res.HasError)
				{
					foreach (var resultProperty in res.Result)
					{
						var property = child.Device.Properties.FirstOrDefault(x => x.Name == resultProperty.Name);
						if (property == null)
						{
							property = new Property()
							{
								Name = resultProperty.Name
							};
							child.Device.Properties.Add(property);
						}
						property.Value = resultProperty.Value;
					}
					child.UpdataConfigurationProperties();
				}
				else
				{
					MessageBoxService.Show("При вызове метода на сервере возникло исключение " + res.Error);
				}
			};

			LoadingService.Close();
		}
		bool CanGetAllDeviceConfigurationParameters()
		{
			foreach (var child in SelectedDevice.Children)
			{
				if ((child != null) && (child.Device.Driver.HasConfigurationProperties))
				{
					return true;
				}
			}
			return false;
		}
		#endregion
		


		public bool IsAlternativeUSB
		{
			get
			{
				return  ((SelectedDevice != null) && (SelectedDevice.Device.Driver.IsAlternativeUSB));
			}
		}

	}
}