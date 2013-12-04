using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Infrastructure.Common;
using XFiresecAPI;

namespace GKProcessor
{
	public static class GKFileInfo
	{
		static byte MinorVersion { get; set; }
		static byte MajorVersion { get; set; }
		static int DescriptorsCount { get; set; }
		static List<byte> Hash1 { get; set; }
 		static List<byte> Hash2 { get; set; }
 		static long FileSize { get; set; }
		static string Date { get; set; }

		public static List<byte> FileBytes { get; private set; }
		public static List<byte> InfoBlock { get; private set; }
		public static void Initialize (byte minorVersion, byte majorVersion, XDeviceConfiguration deviceConfiguration, XDevice gkDevice)
		{
			DescriptorsManager.Create();
			var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice.UID == gkDevice.UID);
			MinorVersion = minorVersion;
			MajorVersion = majorVersion;
			if (gkDatabase != null)
				DescriptorsCount = gkDatabase.Descriptors.Count();
			Hash1 = CreateHash1(deviceConfiguration);
			Hash2 = CreateHash2(deviceConfiguration, gkDevice);
			InitializeFileBytes(deviceConfiguration);
			InitializeInfoBlock();
			Date = DateTime.Now.ToLongDateString();
		}

		public static List<byte> CreateHash1(XDeviceConfiguration deviceConfiguration)
		{
			var configMemoryStream = ZipSerializeHelper.Serialize(deviceConfiguration);
			configMemoryStream.Position = 0;
			var configBytes = configMemoryStream.ToArray();
			return SHA256.Create().ComputeHash(configBytes).ToList();
		}
		public static List<byte> CreateHash2(XDeviceConfiguration deviceConfiguration, XDevice gkDevice)
		{
			var stringBuilder = new StringBuilder();
			foreach (var device in deviceConfiguration.Devices)
			{
				if (device.IsRealDevice && device.GKParent == gkDevice)
					stringBuilder.Append(device.PresentationName).Append("@");
			}
			foreach (var zone in deviceConfiguration.Zones)
			{
				if (zone.GkDatabaseParent == gkDevice)
					stringBuilder.Append(zone.No).Append("@");
			}
			foreach (var direction in deviceConfiguration.Directions)
			{
				if (direction.GkDatabaseParent == gkDevice)
					stringBuilder.Append(direction.No).Append("@");
			}
			return SHA256.Create().ComputeHash(Encoding.GetEncoding(1251).GetBytes(stringBuilder.ToString())).ToList();
		}
		static void InitializeFileBytes(XDeviceConfiguration deviceConfiguration)
		{
			ZipFileConfigurationHelper.SaveToZipFile("configFileToGK", deviceConfiguration);
			var file = File.OpenRead("configFileToGK");
			FileSize = file.Length;
			FileBytes = File.ReadAllBytes(file.Name).ToList();
			file.Close();
		}
		static void InitializeInfoBlock()
		{
			InfoBlock = new List<byte>(256) { MinorVersion, MajorVersion };
			InfoBlock.AddRange(Hash1);
			InfoBlock.AddRange(Hash2);
			while (InfoBlock.Count < 256)
				InfoBlock.Add(0);
		}
	}
}
