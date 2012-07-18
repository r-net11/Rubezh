using GKModule.Converter;
using GKModule.Database;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;
using XFiresecAPI;
using System.IO;
using System.Diagnostics;
using FiresecClient;
using System.Collections;

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
			GetObjectInfoCommand = new RelayCommand(OnGetObjectInfo);
			ExecuteObjectCommand = new RelayCommand(OnExecuteObject);
			EraseWorkingProgramCommand = new RelayCommand(OnEraseWorkingProgram);
			WriteProgramCommand = new RelayCommand(OnWriteProgram);
			GetDeviceParameterCommand = new RelayCommand(OnGetDeviceParameter);
			WriteDeviceParameterCommand = new RelayCommand(OnWriteDeviceParameter);

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
			DatabaseProcessor.Convert();

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
			BinConfigurationCreator.WriteConfig();
		}

		public RelayCommand ConvertToBinaryFileCommand { get; private set; }
		void OnConvertToBinaryFile()
		{
			Directory.Delete(@"D:\GKConfig", true);
			Directory.CreateDirectory(@"D:\GKConfig");
			BinaryFileConverter.Convert3();
		}

		public RelayCommand GetParametersCommand { get; private set; }
		void OnGetParameters()
		{
		}

		public RelayCommand WriteParametersCommand { get; private set; }
		void OnWriteParameters()
		{
		}

		public RelayCommand GetObjectInfoCommand { get; private set; }
		void OnGetObjectInfo()
		{
			var statesViewModel = new StatesViewModel();
			DialogService.ShowModalWindow(statesViewModel);

			for (short i = 1; i < 14; i++)
			{
				var bytes = SendManager.Send(SelectedDevice.Device, 2, 12, 32, BytesHelper.ShortToBytes(i));
				var deviceType = BytesHelper.SubstructShort(bytes, 0);
				var address = BytesHelper.SubstructShort(bytes, 2);
				var serialNo = BytesHelper.SubstructInt(bytes, 4);
				var state = BytesHelper.SubstructInt(bytes, 8);
				Trace.WriteLine(BytesHelper.BytesToString(bytes));

				var driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverTypeNo == deviceType);

				var stateStringBuilder = new StringBuilder();
				var bitArray = new BitArray(new int[1] { state });
				for (int j = 0; j < bitArray.Count; j++)
				{
					var b = bitArray[j];
					if (b)
						stateStringBuilder.Append(j + ", ");
				}

				var message =
					"Тип " + driver.ShortName + "\n" +
					"Адрес " + address + "\n" +
					"Серийный номер " + serialNo + "\n" +
					"Состояние " + stateStringBuilder.ToString();
				MessageBoxService.Show(message);
			}
		}

		public RelayCommand ExecuteObjectCommand { get; private set; }
		void OnExecuteObject()
		{
		}

		public RelayCommand EraseWorkingProgramCommand { get; private set; }
		void OnEraseWorkingProgram()
		{
		}

		public RelayCommand WriteProgramCommand { get; private set; }
		void OnWriteProgram()
		{
		}

		public RelayCommand GetDeviceParameterCommand { get; private set; }
		void OnGetDeviceParameter()
		{
		}

		public RelayCommand WriteDeviceParameterCommand { get; private set; }
		void OnWriteDeviceParameter()
		{
		}
	}
}