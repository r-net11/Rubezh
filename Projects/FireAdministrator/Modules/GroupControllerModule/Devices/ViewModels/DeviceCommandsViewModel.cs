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
using Microsoft.Win32;
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

			ControlTimeCommand = new RelayCommand(OnControlTime, CanControlTime);
			ShowInfoCommand = new RelayCommand(OnShowInfo, CanShowInfo);
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
			var configurationConverter = new ConfigurationConverter2();
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

		bool CanShowInfo()
		{
			return (SelectedDevice != null && (SelectedDevice.Device.Driver.DriverType == XDriverType.KAU ||
				SelectedDevice.Device.Driver.DriverType == XDriverType.GK));
		}
		public RelayCommand ShowInfoCommand { get; private set; }
		void OnShowInfo()
		{
			var deviceInfoViewModel = new DeviceInfoViewModel(SelectedDevice.Device);
			DialogService.ShowModalWindow(deviceInfoViewModel);
		}

		bool CanControlTime()
		{
			return (SelectedDevice != null && SelectedDevice.Device.Driver.DriverType == XDriverType.GK);
		}
		public RelayCommand ControlTimeCommand { get; private set; }
		void OnControlTime()
		{
			var deviceTimeViewModel = new DeviceTimeViewModel(SelectedDevice.Device);
			DialogService.ShowModalWindow(deviceTimeViewModel);
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
			BinConfigurationWriter.WriteConfig();
		}

		public RelayCommand ConvertToBinaryFileCommand { get; private set; }
		void OnConvertToBinaryFile()
		{
			var saveDialog = new SaveFileDialog();
			if (saveDialog.ShowDialog().Value)
			{
				var x = saveDialog.FileName;
			}
			return;

			Directory.Delete(@"D:\GKConfig", true);
			Directory.CreateDirectory(@"D:\GKConfig");
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
			MessageBoxService.Show("Функия пока не реализована");
		}
		bool CanUpdateFirmwhare()
		{
			return ((SelectedDevice != null) || (SelectedDevice.Driver.DriverType == XDriverType.KAU));
		}
	}
}