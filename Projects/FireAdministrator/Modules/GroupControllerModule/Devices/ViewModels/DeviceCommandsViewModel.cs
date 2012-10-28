using System.Collections.Generic;
using System.IO;
using System.Text;
using Common.GK;
using GKModule.Converter;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecClient;
using FiresecAPI.Models;

namespace GKModule.Models
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		DevicesViewModel _devicesViewModel;

		public DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
		{
			ConvertFromFiresecCommand = new RelayCommand(OnConvertFromFiresec);
			ConvertToBinCommand = new RelayCommand(OnConvertToBin);
			ConvertToBinaryFileCommand = new RelayCommand(OnConvertToBinaryFile);
            WriteConfigCommand = new RelayCommand(OnWriteConfig, CanWriteConfig);

			SynchroniseTimeCommand = new RelayCommand(OnSynchroniseTime, CanSynchroniseTime);
			ReadJournalCommand = new RelayCommand(OnReadJournal, CanReadJournal);
			GetAllParametersCommand = new RelayCommand(OnGetAllParameters);
			SetAllParametersCommand = new RelayCommand(OnSetAllParameters);
            GetSingleParametersCommand = new RelayCommand(GetSingleParameters, CanGetSetSingleParameter);
            SetSingleParametersCommand = new RelayCommand(OnSetSingleParameters, CanGetSetSingleParameter);
			UpdateFirmwhareCommand = new RelayCommand(OnUpdateFirmwhare, CanUpdateFirmwhare);

			_devicesViewModel = devicesViewModel;
		}

		public DeviceViewModel SelectedDevice
		{
			get { return _devicesViewModel.SelectedDevice; }
		}

		public RelayCommand ConvertFromFiresecCommand { get; private set; }
		void OnConvertFromFiresec()
		{
			var configurationConverter = new ConfigurationConverter();
			configurationConverter.Convert();

			DevicesViewModel.Current.Initialize();
			ZonesViewModel.Current.Initialize();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand ConvertToBinCommand { get; private set; }
		void OnConvertToBin()
		{
			DatabaseManager.Convert();
			var deviceConverterViewModel = new DatabasesViewModel();
			DialogService.ShowModalWindow(deviceConverterViewModel);
		}

		string BytesToString(List<byte> bytes)
		{
			var stringBuilder = new StringBuilder();
			foreach (var b in bytes)
			{
				stringBuilder.Append(b + " ");
			}
			return stringBuilder.ToString();
		}

		bool CanSynchroniseTime()
		{
			return (SelectedDevice != null && SelectedDevice.Device.Driver.DriverType == XDriverType.GK);
		}
		public RelayCommand SynchroniseTimeCommand { get; private set; }
		void OnSynchroniseTime()
		{
			DeviceTimeHelper.Write(SelectedDevice.Device);
		}

		bool CanReadJournal()
		{
			return (SelectedDevice != null && SelectedDevice.Device.Driver.DriverType == XDriverType.GK);
		}
		public RelayCommand ReadJournalCommand { get; private set; }
		void OnReadJournal()
		{
			var journalViewModel = new JournalViewModel(SelectedDevice.Device);
			DialogService.ShowModalWindow(journalViewModel);
		}

		public RelayCommand WriteConfigCommand { get; private set; }
		void OnWriteConfig()
		{
			if (ValidateConfiguration())
			{
				BinConfigurationWriter.WriteConfig();
			}
			else
			{
				if (MessageBoxService.ShowQuestion("Конфигурация содержит ошибки. Продолжить?") == System.Windows.MessageBoxResult.Yes)
				{
					BinConfigurationWriter.WriteConfig();
				}
			}
		}
        bool CanWriteConfig()
        {
            return FiresecManager.CheckPermission(PermissionType.Adm_WriteDeviceConfig);
        }

		public RelayCommand ConvertToBinaryFileCommand { get; private set; }
		void OnConvertToBinaryFile()
		{
			Directory.Delete(@"C:\GKConfig", true);
			Directory.CreateDirectory(@"C:\GKConfig");
			BinaryFileConverter.Convert();
		}

		public RelayCommand GetAllParametersCommand { get; private set; }
		void OnGetAllParameters()
		{
			ParametersHelper.GetAllParameters();
		}

		public RelayCommand SetAllParametersCommand { get; private set; }
		void OnSetAllParameters()
		{
			ParametersHelper.SetAllParameters();
		}

		public RelayCommand GetSingleParametersCommand { get; private set; }
		void GetSingleParameters()
		{
			ParametersHelper.GetSingleParameter(SelectedDevice.Device);
		}

        public RelayCommand SetSingleParametersCommand { get; private set; }
        void OnSetSingleParameters()
		{
            ParametersHelper.SetSingleParameter(SelectedDevice.Device);
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
			MessageBoxService.Show("Функция не реализована");
		}
		bool CanUpdateFirmwhare()
		{
			return (SelectedDevice != null && SelectedDevice.Driver.DriverType == XDriverType.KAU && FiresecManager.CheckPermission(PermissionType.Adm_ChangeDevicesSoft));
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