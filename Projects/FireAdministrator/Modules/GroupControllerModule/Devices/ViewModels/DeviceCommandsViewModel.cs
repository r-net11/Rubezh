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

			SynchroniseTimeCommand = new RelayCommand(OnSynchroniseTime, CanSynchroniseTime);
			ReadJournalCommand = new RelayCommand(OnReadJournal, CanReadJournal);
			WriteConfigCommand = new RelayCommand(OnWriteConfig);
			GetParametersCommand = new RelayCommand(OnGetParameters);
			WriteParametersCommand = new RelayCommand(OnWriteParameters);
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
			ServiceFactory.SaveService.XDevicesChanged = true;
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
			//if (ValidateConfiguration())
			{
				BinConfigurationWriter.WriteConfig();
			}
		}

		public RelayCommand ConvertToBinaryFileCommand { get; private set; }
		void OnConvertToBinaryFile()
		{
			Directory.Delete(@"C:\GKConfig", true);
			Directory.CreateDirectory(@"C:\GKConfig");
			BinaryFileConverter.Convert();
		}

		public RelayCommand GetParametersCommand { get; private set; }
		void OnGetParameters()
		{
			ParametersHelper.GetParameters();
		}

		public RelayCommand WriteParametersCommand { get; private set; }
		void OnWriteParameters()
		{
			ParametersHelper.SetParameters();
		}

		public RelayCommand UpdateFirmwhareCommand { get; private set; }
		void OnUpdateFirmwhare()
		{
			MessageBoxService.Show("Функция не реализована");
		}
		bool CanUpdateFirmwhare()
		{
			return ((SelectedDevice != null) && (SelectedDevice.Driver.DriverType == XDriverType.KAU));
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