using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecClient;
using GKModule.Converter;
using GKModule.Diagnostics;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			ConvertToBinCommand = new RelayCommand(OnConvertToBin);
			ConvertFromFiresecCommand = new RelayCommand(OnConvertFromFiresec);
			ConvertToFiresecCommand = new RelayCommand(OnConvertToFiresec);
			GoToTechnologicalCommand = new RelayCommand(OnGoToTechnological);
			GoToWorkRegimeCommand = new RelayCommand(OnGoToWorkRegime);
			CreateTestZonesCommand = new RelayCommand(OnCreateTestZones);
			ConvertShleifCommand = new RelayCommand(OnConvertShleif);
			WriteConfigFileToGKCommand = new RelayCommand(OnWriteConfigFileToGK);
			ReadConfigFileFromGKCommand = new RelayCommand(OnReadConfigFileFromGK);
		}

		public RelayCommand ConvertFromFiresecCommand { get; private set; }
		void OnConvertFromFiresec()
		{
			var configurationConverter = new FiresecToGKConverter();
			configurationConverter.Convert();

			DevicesViewModel.Current.Initialize();
			ZonesViewModel.Current.Initialize();
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand ConvertToBinCommand { get; private set; }
		void OnConvertToBin()
		{
			DatabaseManager.Convert();
			var databasesViewModel = new DatabasesViewModel();
			DialogService.ShowModalWindow(databasesViewModel);
		}

		public RelayCommand ConvertToFiresecCommand { get; private set; }
		void OnConvertToFiresec()
		{
			var gkToFiresecConverter = new GKToFiresecConverter();
			gkToFiresecConverter.Convert();
			ServiceFactory.SaveService.FSChanged = true;
		}

		public RelayCommand GoToTechnologicalCommand { get; private set; }
		void OnGoToTechnological()
		{
			var device = XManager.Devices.FirstOrDefault(x => x.DriverType == XFiresecAPI.XDriverType.GK);
			var sendResult = SendManager.Send(device, 0, 14, 0, null, device.DriverType == XDriverType.GK);
		}

		public RelayCommand GoToWorkRegimeCommand { get; private set; }
		void OnGoToWorkRegime()
		{
			var device = XManager.Devices.FirstOrDefault(x => x.DriverType == XFiresecAPI.XDriverType.GK);
			SendManager.Send(device, 0, 11, 0, null, device.DriverType == XDriverType.GK);
		}

		public RelayCommand CreateTestZonesCommand { get; private set; }
		void OnCreateTestZones()
		{
			var device = XManager.Devices.FirstOrDefault(x => x.DriverType == XDriverType.HandDetector);
			for (int i = 0; i < 20000; i++)
			{
				var zone = new XZone()
				{
					No = 10000 + i,
					Name = "TestZone_" + i
				};
				XManager.Zones.Add(zone);
				device.ZoneUIDs.Add(zone.UID);
			}
			ServiceFactory.SaveService.GKChanged = true;
		}

		public RelayCommand ConvertShleifCommand { get; private set; }
		void OnConvertShleif()
		{
			PatchShleif(XDriverType.KAU, XDriverType.KAU_Shleif);
			PatchShleif(XDriverType.RSR2_KAU, XDriverType.RSR2_KAU_Shleif);
			XManager.UpdateConfiguration();
			ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
		}

		void PatchShleif(XDriverType kauDriverType, XDriverType shleifDriverType)
		{
			var kauDevices = XManager.Devices.Where(x => x.DriverType == kauDriverType);
			foreach (var kauDevice in kauDevices)
			{
				var driver = XManager.Drivers.FirstOrDefault(x => x.DriverType == shleifDriverType);
				var shleifDevices = new List<XDevice>();
				for (int i = 0; i < 8; i++)
				{
					var device = new XDevice()
					{
						IntAddress = (byte)(i + 1),
						Driver = driver,
						DriverUID = driver.UID
					};
					shleifDevices.Add(device);
				}
				foreach (var device in kauDevice.Children)
				{
					if (device.DriverType != XDriverType.KAUIndicator)
					{
						var shleifDevice = shleifDevices.FirstOrDefault(x => x.IntAddress == device.ShleifNo);
						shleifDevice.Children.Add(device);
					}
				}
				kauDevice.Children.RemoveAll(x => x.DriverType != XDriverType.KAUIndicator);
				kauDevice.Children.AddRange(shleifDevices);
			}
		}

		public RelayCommand WriteConfigFileToGKCommand { get; private set; }
		void OnWriteConfigFileToGK()
		{
			var gkDevice = XManager.Devices.FirstOrDefault(y => y.DriverType == XDriverType.GK);
			BinConfigurationWriter.GoToTechnologicalRegime(gkDevice);
			var folderName = AppDataFolderHelper.GetLocalFolder("Administrator/Configuration");
			var configFileName = Path.Combine(folderName, "Config.fscp");
			if (!File.Exists(configFileName))
				return;
			var bytesList = File.ReadAllBytes(configFileName).ToList();
			var tempBytes = new List<List<byte>>();
			var sendResult = SendManager.Send(gkDevice, 0, 21, 0);
			for (int i = 0; i < bytesList.Count(); i += 256)
			{
				var bytesBlock = BitConverter.GetBytes((uint)(i / 256 + 1)).ToList();
				bytesBlock.AddRange(bytesList.GetRange(i, Math.Min(256, bytesList.Count - i)));
				tempBytes.Add(bytesBlock.GetRange(4, bytesBlock.Count - 4));
				SendManager.Send(gkDevice, (ushort)bytesBlock.Count(), 22, 0, bytesBlock);
			}
			var endBlock = BitConverter.GetBytes((uint)(bytesList.Count() / 256 + 1)).ToList();
			SendManager.Send(gkDevice, 0, 22, 0, endBlock);
			//BytesHelper.BytesToFile("output.txt", tempBytes);
			//GoToWorkingRegime(gkDevice);
		}

		public RelayCommand ReadConfigFileFromGKCommand { get; private set; }
		void OnReadConfigFileFromGK()
		{
			var gkDevice = XManager.Devices.FirstOrDefault(y => y.DriverType == XDriverType.GK);
			BinConfigurationWriter.GoToTechnologicalRegime(gkDevice);
			var bytesList = new List<List<byte>>();
			var allbytes = new List<byte>();
			uint i = 1;
			while (true)
			{
				var data = new List<byte>(BitConverter.GetBytes(i++));
				var sendResult = SendManager.Send(gkDevice, 4, 23, 256, data);
				bytesList.Add(sendResult.Bytes);
				allbytes.AddRange(sendResult.Bytes);
				if (sendResult.HasError || sendResult.Bytes.Count() < 256)
					break;
			}
			//BytesHelper.BytesToFile("input.txt", bytesList);
			ByteArrayToFile("gkConfig.fscp", allbytes.ToArray());
			//GoToWorkingRegime(gkDevice);
		}

		public bool ByteArrayToFile(string fileName, byte[] byteArray)
		{
			try
			{
				var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
				fileStream.Write(byteArray, 0, byteArray.Length);
				fileStream.Close();
				return true;
			}
			catch (Exception exception)
			{
				Console.WriteLine("Exception caught in process: {0}",exception);
			}
			return false;
		}

	}
}