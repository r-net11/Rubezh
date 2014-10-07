using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace GKModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			TestCommand = new RelayCommand(OnTest);
			GoToTechnologicalCommand = new RelayCommand(OnGoToTechnological);
			GoToWorkRegimeCommand = new RelayCommand(OnGoToWorkRegime);
			WriteConfigFileToGKCommand = new RelayCommand(OnWriteConfigFileToGK);
			ReadConfigFileFromGKCommand = new RelayCommand(OnReadConfigFileFromGK);
			CompareHashesCommand = new RelayCommand(OnCompareHashes, CanCompareHashes);
		}

		public DescriptorsViewModel DatabasesViewModel { get; private set; }

		public RelayCommand CompareHashesCommand { get; private set; }
		void OnCompareHashes()
		{
			var localHash1 = GKFileInfo.CreateHash1(GKManager.DeviceConfiguration, DevicesViewModel.Current.SelectedDevice.Device);
			var localHash2 = GKFileInfo.CreateHash2(GKManager.DeviceConfiguration);
			var gkFileReaderWriter = new GKFileReaderWriter();
			var infoBlock = gkFileReaderWriter.ReadInfoBlock(DevicesViewModel.Current.SelectedDevice.Device);
			if (gkFileReaderWriter.Error != null)
				{ MessageBoxService.ShowError(gkFileReaderWriter.Error); return; }
			var remoteHash1 = infoBlock.Hash1;
			var remoteHash2 = infoBlock.Hash2;

			var message = new StringBuilder();
			message.Append(localHash1.SequenceEqual(remoteHash1) ? "Хеш1 совпадает\n" : "Хеш1 НЕ совпадает\n");
			message.Append(localHash2.SequenceEqual(remoteHash2) ? "Хеш2 совпадает" : "Хеш2 НЕ совпадает");
			MessageBoxService.ShowWarning(message.ToString(), "Сравнение хешей");
		}

		bool CanCompareHashes()
		{
			try
			{
				return 
					DevicesViewModel.Current.SelectedDevice != null &&
					DevicesViewModel.Current.SelectedDevice.Device.DriverType == GKDriverType.GK;
			}
			catch
			{
				return false;
			}
		}
		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			foreach (var device in GKManager.Devices)
			{
				var newUID = GuidHelper.CreateOn(device.UID, 0);
				Trace.WriteLine(device.UID + "	" + newUID);
			}
			return;

			var baseUID = GKManager.Devices.FirstOrDefault().UID;
			return;
		}

		public RelayCommand GoToTechnologicalCommand { get; private set; }
		void OnGoToTechnological()
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			var sendResult = SendManager.Send(device, 0, 14, 0, null, device.DriverType == GKDriverType.GK);
		}

		public RelayCommand GoToWorkRegimeCommand { get; private set; }
		void OnGoToWorkRegime()
		{
			var device = GKManager.Devices.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			SendManager.Send(device, 0, 11, 0, null, device.DriverType == GKDriverType.GK);
		}

		public RelayCommand WriteConfigFileToGKCommand { get; private set; }
		void OnWriteConfigFileToGK()
		{
			var gkControllerDevice = GKManager.Devices.FirstOrDefault(y => y.DriverType == GKDriverType.GK);
			DeviceBytesHelper.GoToTechnologicalRegime(gkControllerDevice, new GKProgressCallback());
			var folderName = AppDataFolderHelper.GetLocalFolder("Administrator/Configuration");
			var configFileName = Path.Combine(folderName, "Config.fscp");
			if (!File.Exists(configFileName))
				return;
			var bytesList = File.ReadAllBytes(configFileName).ToList();
			var tempBytes = new List<List<byte>>();
			var sendResult = SendManager.Send(gkControllerDevice, 0, 21, 0);
			for (int i = 0; i < bytesList.Count(); i += 256)
			{
				var bytesBlock = BitConverter.GetBytes((uint)(i / 256 + 1)).ToList();
				bytesBlock.AddRange(bytesList.GetRange(i, Math.Min(256, bytesList.Count - i)));
				tempBytes.Add(bytesBlock.GetRange(4, bytesBlock.Count - 4));
				SendManager.Send(gkControllerDevice, (ushort)bytesBlock.Count(), 22, 0, bytesBlock);
			}
			var endBlock = BitConverter.GetBytes((uint)(bytesList.Count() / 256 + 1)).ToList();
			SendManager.Send(gkControllerDevice, 0, 22, 0, endBlock);
		}

		public RelayCommand ReadConfigFileFromGKCommand { get; private set; }
		void OnReadConfigFileFromGK()
		{
			var gkControllerDevice = GKManager.Devices.FirstOrDefault(y => y.DriverType == GKDriverType.GK);
			var bytesList = new List<List<byte>>();
			var allbytes = new List<byte>();
			uint i = 1;
			while (true)
			{
				var data = new List<byte>(BitConverter.GetBytes(i++));
				var sendResult = SendManager.Send(gkControllerDevice, 4, 23, 256, data);
				bytesList.Add(sendResult.Bytes);
				allbytes.AddRange(sendResult.Bytes);
				if (sendResult.HasError || sendResult.Bytes.Count() < 256)
					break;
			}
			//BytesHelper.BytesToFile("input.txt", bytesList);
			ByteArrayToFile("gkConfig.fscp", allbytes.ToArray());
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