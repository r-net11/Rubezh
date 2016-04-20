using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RubezhAPI.GK;
using Infrastructure.Common.Windows;
using RubezhAPI;

namespace GKProcessor
{
	public class GKFileInfo
	{
		public static string Error { get; private set; }
		public byte MinorVersion { get; private set; }
		public byte MajorVersion { get; private set; }
		public List<byte> Hash1 { get; set; }
		public int DescriptorsCount { get; set; }
		public long FileSize { get; set; }
		public DateTime Date { get; set; }

		public List<byte> FileBytes { get; private set; }
		public List<byte> InfoBlock { get; private set; }

		public void Initialize(GKDevice gkControllerDevice)
		{
			Date = DateTime.Now;
			var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice.UID == gkControllerDevice.UID);
			MinorVersion = (byte)GKManager.DeviceConfiguration.Version.MinorVersion;
			MajorVersion = (byte)GKManager.DeviceConfiguration.Version.MajorVersion;
			if (gkDatabase != null)
				DescriptorsCount = gkDatabase.Descriptors.Count();
			Hash1 = CreateHash1(gkControllerDevice);
			InitializeFileBytes();
			InitializeInfoBlock();
		}
		public static List<byte> CreateHash1(GKDevice gkControllerDevice)
		{
			var deviceConfiguration = GKManager.DeviceConfiguration;
			var stringBuilder = new StringBuilder();
			stringBuilder.Append("devices:");

			foreach (var device in deviceConfiguration.Devices.Where(x => x.IsRealDevice && x.GkDatabaseParent == gkControllerDevice))
			{
				stringBuilder.Append(device.PresentationName).Append("@");
			}
			stringBuilder.Append("zones:");
			foreach (var zone in deviceConfiguration.Zones.Where(x => x.GkDatabaseParent == gkControllerDevice))
			{
				stringBuilder.Append(zone.PresentationName).Append("@");
			}
			stringBuilder.Append("directions:");
			foreach (var direction in deviceConfiguration.Directions.Where(x => x.GkDatabaseParent == gkControllerDevice))
			{
				stringBuilder.Append(direction.PresentationName).Append("@");
			}
			stringBuilder.Append("pumpStations:");
			foreach (var pumpStation in deviceConfiguration.PumpStations.Where(x => x.GkDatabaseParent == gkControllerDevice))
			{
				stringBuilder.Append(pumpStation.PresentationName).Append("@");
				if (pumpStation.NSDevices != null)
				{
					stringBuilder.Append("nsDevices:");
					foreach (var nsDevice in pumpStation.NSDevices.Where(x => x.GKParent == gkControllerDevice))
					{
						stringBuilder.Append(nsDevice.PresentationName).Append("@");
					}
				}
			}
			stringBuilder.Append("mpts:");
			foreach (var mpt in deviceConfiguration.MPTs.Where(x => x.GkDatabaseParent == gkControllerDevice))
			{
				stringBuilder.Append(mpt.PresentationName).Append("@");
				if (mpt.MPTDevices != null)
				{
					stringBuilder.Append("nsDevices:");
					foreach (var mptDevice in mpt.MPTDevices.Where(x => x.Device != null && x.Device.GKParent == gkControllerDevice))
					{
						stringBuilder.Append(mptDevice.Device.PresentationName).Append("@");
					}
				}
			}
			stringBuilder.Append("delays:");
			foreach (var delay in deviceConfiguration.Delays.Where(x => x.GkDatabaseParent == gkControllerDevice))
			{
				stringBuilder.Append(delay.PresentationName).Append("@");
			}
			stringBuilder.Append("guardZones:");
			foreach (var guardZone in deviceConfiguration.GuardZones.Where(x => x.GkDatabaseParent == gkControllerDevice))
			{
				stringBuilder.Append(guardZone.PresentationName).Append("@");
			}
			stringBuilder.Append("codes:");
			foreach (var code in deviceConfiguration.Codes.Where(x => x.GkDatabaseParent == gkControllerDevice))
			{
				stringBuilder.Append(code.PresentationName).Append("@");
			}
			stringBuilder.Append("door:");
			foreach (var door in deviceConfiguration.Doors.Where(x => x.GkDatabaseParent == gkControllerDevice))
			{
				stringBuilder.Append(door.PresentationName).Append("@");
			}
			return SHA256.Create().ComputeHash(Encoding.GetEncoding(1251).GetBytes(stringBuilder.ToString())).ToList();
		}
		void InitializeFileBytes()
		{
			FileStream fileStream;
			if (GKManager.DeviceConfiguration.OnlyGKDeviceConfiguration)
				fileStream = File.OpenRead(Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "GKDeviceConfiguration.fscp"));
			else
				fileStream = File.OpenRead(Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp"));
			FileSize = fileStream.Length;
			FileBytes = File.ReadAllBytes(fileStream.Name).ToList();
			fileStream.Close();
		}
		void InitializeInfoBlock()
		{
			InfoBlock = new List<byte>(256) { MinorVersion, MajorVersion };
			InfoBlock.AddRange(Hash1);
			InfoBlock.AddRange(BitConverter.GetBytes(DescriptorsCount));
			InfoBlock.AddRange(BitConverter.GetBytes(FileSize));
			InfoBlock.AddRange(BitConverter.GetBytes(Date.Ticks));
			while (InfoBlock.Count < 256)
				InfoBlock.Add(0);
		}

		public static GKFileInfo BytesToGKFileInfo(List<byte> bytes)
		{
			Error = null;
			try
			{
				return new GKFileInfo
				{
					InfoBlock = bytes,
					MinorVersion = bytes[0],
					MajorVersion = bytes[1],
					Hash1 = bytes.GetRange(2, 32),
					DescriptorsCount = BitConverter.ToInt32(bytes.GetRange(34, 4).ToArray(), 0),
					FileSize = BitConverter.ToInt64(bytes.GetRange(38, 8).ToArray(), 0),
					Date = DateTime.FromBinary(BitConverter.ToInt64(bytes.GetRange(46, 8).ToArray(), 0)),
					FileBytes = new List<byte>()
				};
			}
			catch
			{
				Error = "Информационный блок поврежден";
				return null;
			}
		}

		public static bool CompareHashes(List<byte> Hash1, List<byte> Hash2)
		{
			return Hash1.SequenceEqual(Hash2);
		}
	}
}