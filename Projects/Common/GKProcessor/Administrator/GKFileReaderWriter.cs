using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKProcessor
{
	public static class GKFileReaderWriter
	{
		public static string ParsingError { get; set; }
		public static XDeviceConfiguration ReadConfigFileFromGK()
		{
			var gkDevice = XManager.Devices.FirstOrDefault(y => y.DriverType == XDriverType.GK);
			if (gkDevice == null)
				{ ParsingError = "Ошибка конфигурации, не найден ГК в конфигурации"; return null; }
			var bytesList = new List<List<byte>>();
			var allbytes = new List<byte>();
			uint i = 1;
			LoadingService.Show("Чтение конфигурационного файла из " + gkDevice.PresentationName);
			while (true)
			{
				LoadingService.DoStep("Чтение объекта " + i);
				var data = new List<byte>(BitConverter.GetBytes(i++));
				var sendResult = SendManager.Send(gkDevice, 4, 23, 256, data);
				if(sendResult.HasError)
					{ ParsingError = "Невозможно прочитать блок данных " + i; break; }
				bytesList.Add(sendResult.Bytes);
				allbytes.AddRange(sendResult.Bytes);
				if (sendResult.Bytes.Count() < 256)
					break;
			}
			var deviceConfiguration = ZipFileConfigurationHelper.LoadFromZipFile(new MemoryStream(allbytes.ToArray()));
			deviceConfiguration.Devices.ForEach(x => x.Driver = XManager.Drivers.FirstOrDefault(z => z.UID == x.DriverUID));
			foreach (var zone in deviceConfiguration.Zones)
			{
				var device = deviceConfiguration.Devices.FirstOrDefault(x => x.ZoneUIDs.Contains(zone.UID));
				if (device != null)
					zone.GkDatabaseParent = device.GKParent;
			}

			foreach (var device in deviceConfiguration.Devices)
			{
				InvalidateOneLogic(deviceConfiguration, device, device.DeviceLogic);
				if (device.NSLogic != null)
					InvalidateOneLogic(deviceConfiguration, device, device.NSLogic);
			}

			deviceConfiguration.Update();
			UpdateConfigurationHelper.UpdateGKPredefinedName(gkDevice);
			LoadingService.Close();
			return deviceConfiguration;
		}

		public static void WriteConfigFileToGK()
		{
			var gkDevice = XManager.Devices.FirstOrDefault(x => x.DriverType == XDriverType.GK);
			var folderName = AppDataFolderHelper.GetLocalFolder("Administrator/Configuration");
			var configFileName = Path.Combine(folderName, "fileToGk.fscp");

			ZipFileConfigurationHelper.SaveToZipFile(configFileName, XManager.DeviceConfiguration);
			if (!File.Exists(configFileName))
				return;
			var bytesList = File.ReadAllBytes(configFileName).ToList();
			var tempBytes = new List<List<byte>>();
			var sendResult = SendManager.Send(gkDevice, 0, 21, 0);
			if (sendResult.HasError)
				{ ParsingError = "Невозможно начать процедуру записи "; return; }
			LoadingService.Show("Запись конфигурационного файла в " + gkDevice.PresentationName, null, bytesList.Count);
			for (int i = 0; i < bytesList.Count(); i += 256)
			{
				LoadingService.SaveDoStep("Запись объекта " + i + 1);
				var bytesBlock = BitConverter.GetBytes((uint)(i / 256 + 1)).ToList();
				bytesBlock.AddRange(bytesList.GetRange(i, Math.Min(256, bytesList.Count - i)));
				tempBytes.Add(bytesBlock.GetRange(4, bytesBlock.Count - 4));
				sendResult = SendManager.Send(gkDevice, (ushort)bytesBlock.Count(), 22, 0, bytesBlock);
				if (sendResult.HasError)
					{ ParsingError = "Невозможно записать блок данных " + i; return; }
			}
			var endBlock = BitConverter.GetBytes((uint)(bytesList.Count() / 256 + 1)).ToList();
			sendResult = SendManager.Send(gkDevice, 0, 22, 0, endBlock);
			if (sendResult.HasError)
				{ ParsingError = "Невозможно завершить запись файла "; }
		}

		public static void InvalidateOneLogic(XDeviceConfiguration deviceConfiguration, XDevice device, XDeviceLogic deviceLogic)
		{
			var devices = deviceConfiguration.Devices;
			var zones = deviceConfiguration.Zones;
			var directions = deviceConfiguration.Directions;
			ClearAllReferences(devices, zones, directions);

			var clauses = new List<XClause>();
			foreach (var clause in deviceLogic.Clauses)
			{
				clause.Devices = new List<XDevice>();
				clause.Zones = new List<XZone>();
				clause.Directions = new List<XDirection>();

				var zoneUIDs = new List<Guid>();
				foreach (var zoneUID in clause.ZoneUIDs)
				{
					var zone = zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						zoneUIDs.Add(zoneUID);
						clause.Zones.Add(zone);
						zone.DevicesInLogic.Add(device);
					}
				}
				clause.ZoneUIDs = zoneUIDs;

				var deviceUIDs = new List<Guid>();
				foreach (var deviceUID in clause.DeviceUIDs)
				{
					var clauseDevice = devices.FirstOrDefault(x => x.UID == deviceUID);
					if (clauseDevice != null && !clauseDevice.IsNotUsed)
					{
						deviceUIDs.Add(deviceUID);
						clause.Devices.Add(clauseDevice);
						clauseDevice.DevicesInLogic.Add(device);
					}
				}
				clause.DeviceUIDs = deviceUIDs;

				var directionUIDs = new List<Guid>();
				foreach (var directionUID in clause.DirectionUIDs)
				{
					var direction = directions.FirstOrDefault(x => x.UID == directionUID);
					if (direction != null)
					{
						directionUIDs.Add(directionUID);
						clause.Directions.Add(direction);
						direction.OutputDevices.Add(device);
						device.Directions.Add(direction);
					}
				}
				clause.DirectionUIDs = directionUIDs;

				if (clause.Zones.Count > 0 || clause.Devices.Count > 0 || clause.Directions.Count > 0)
					clauses.Add(clause);
			}
			deviceLogic.Clauses = clauses;
		}

		static void ClearAllReferences(List<XDevice> devices, List<XZone> zones,  List<XDirection> directions )
		{
			foreach (var device in devices)
			{
				device.Zones = new List<XZone>();
				device.Directions = new List<XDirection>();
				device.NSDirections = new List<XDirection>();
				device.DevicesInLogic = new List<XDevice>();
			}
			foreach (var zone in zones)
			{
				zone.Devices = new List<XDevice>();
				zone.Directions = new List<XDirection>();
				zone.DevicesInLogic = new List<XDevice>();
			}
			foreach (var direction in directions)
			{
				direction.InputZones = new List<XZone>();
				direction.InputDevices = new List<XDevice>();
				direction.OutputDevices = new List<XDevice>();
				direction.NSDevices = new List<XDevice>();
			}
		}
	}
}
