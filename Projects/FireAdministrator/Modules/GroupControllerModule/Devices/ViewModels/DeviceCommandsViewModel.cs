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

namespace GKModule.Models
{
	public class DeviceCommandsViewModel : BaseViewModel
	{
		DevicesViewModel _devicesViewModel;

		public DeviceCommandsViewModel(DevicesViewModel devicesViewModel)
		{
			ConvertFromFiresecCommand = new RelayCommand(OnConvertFromFiresec);
			ConvertToBinCommand = new RelayCommand(OnConvertToBin);

			ControlTimeCommand = new RelayCommand(OnControlTime, CanControlTime);
			ShowInfoCommand = new RelayCommand(OnShowInfo, CanShowInfo);
			ReadJournalCommand = new RelayCommand(OnReadJournal, CanReadJournal);
			WriteConfigCommand = new RelayCommand(OnWriteConfig, CanWriteConfig);
			ConvertToBinaryFileCommand = new RelayCommand(OnConvertToBinaryFile, CanConvertToBinaryFile);

			GetParametersCommand = new RelayCommand(OnGetParameters);
			WriteParametersCommand = new RelayCommand(OnWriteParameters);
			StartWorkingProgramCommand = new RelayCommand(OnStartWorkingProgram);
			GetObjectInfoCommand = new RelayCommand(OnGetObjectInfo);
			ExecuteObjectCommand = new RelayCommand(OnExecuteObject);
			GoToTechnologicalProgrammCommand = new RelayCommand(OnGoToTechnologicalProgramm);
			EraseDatabaseCommand = new RelayCommand(OnEraseDatabase);
			EraseWorkingProgramCommand = new RelayCommand(OnEraseWorkingProgram);
			WriteDatabaseCommand = new RelayCommand(OnWriteDatabase);
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

		bool CanWriteConfig()
		{
			return (SelectedDevice != null && SelectedDevice.Device.Driver.DriverType == XDriverType.GK);
		}
		public RelayCommand WriteConfigCommand { get; private set; }
		void OnWriteConfig()
		{
			DatabaseProcessor.Convert();

			var gkDatabase = DatabaseProcessor.DatabaseCollection.GkDatabases.First();
			foreach (var binaryObject in gkDatabase.BinaryObjects)
			{
				var bytes = BinaryFileConverter.CreateDescriptor(binaryObject, true);
				CommandManager.Send(SelectedDevice.Device, (short)(3 + bytes.Count()), 17, 0, bytes);
			}
			var endBytes = BinaryFileConverter.CreateEndDescriptor((short)gkDatabase.BinaryObjects.Count);
			CommandManager.Send(SelectedDevice.Device, 3 + 2, 17, 0, endBytes);
		}

		bool CanConvertToBinaryFile()
		{
			return (SelectedDevice != null && SelectedDevice.Device.Driver.DriverType == XDriverType.GK);
		}
		public RelayCommand ConvertToBinaryFileCommand { get; private set; }
		void OnConvertToBinaryFile()
		{
			Directory.Delete(@"D:\GKConfig", true);
			Directory.CreateDirectory(@"D:\GKConfig");
			BinaryFileConverter.Convert3();
			//BinaryFileConverter.Convert1();
			//BinaryFileConverter.Convert2();
		}

		public RelayCommand GetParametersCommand { get; private set; }
		void OnGetParameters()
		{
		}

		public RelayCommand WriteParametersCommand { get; private set; }
		void OnWriteParameters()
		{
		}

		public RelayCommand StartWorkingProgramCommand { get; private set; }
		void OnStartWorkingProgram()
		{
		}

		public RelayCommand GetObjectInfoCommand { get; private set; }
		void OnGetObjectInfo()
		{
		}

		public RelayCommand ExecuteObjectCommand { get; private set; }
		void OnExecuteObject()
		{
		}

		public RelayCommand GoToTechnologicalProgrammCommand { get; private set; }
		void OnGoToTechnologicalProgramm()
		{
		}

		public RelayCommand EraseDatabaseCommand { get; private set; }
		void OnEraseDatabase()
		{
		}

		public RelayCommand EraseWorkingProgramCommand { get; private set; }
		void OnEraseWorkingProgram()
		{
		}

		public RelayCommand WriteDatabaseCommand { get; private set; }
		void OnWriteDatabase()
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