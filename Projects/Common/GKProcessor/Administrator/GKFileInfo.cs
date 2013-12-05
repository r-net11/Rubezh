using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using XFiresecAPI;

namespace GKProcessor
{
	public class GKFileInfo
	{
		public byte MinorVersion { get; private set; }
		public byte MajorVersion { get; private set; }
		public List<byte> Hash1 { get; set; }
		public List<byte> Hash2 { get; set; }
		public int DescriptorsCount { get; set; }
		public long FileSize { get; set; }
		public DateTime Date { get; set; }

		public List<byte> FileBytes { get; private set; }
		public List<byte> InfoBlock { get; private set; }
		public void Initialize (XDeviceConfiguration deviceConfiguration, XDevice gkDevice)
		{
			DescriptorsManager.Create();
			Date = DateTime.Now;
			var gkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault(x => x.RootDevice.UID == gkDevice.UID);
			MinorVersion = (byte)deviceConfiguration.Version.MinorVersion;
			MajorVersion = (byte)deviceConfiguration.Version.MajorVersion;
			if (gkDatabase != null)
				DescriptorsCount = gkDatabase.Descriptors.Count();
			Hash1 = CreateHash1(deviceConfiguration);
			Hash2 = CreateHash2(deviceConfiguration, gkDevice);
			InitializeFileBytes(deviceConfiguration);
			InitializeInfoBlock();
		}

		public static List<byte> CreateHash1(XDeviceConfiguration deviceConfiguration)
		{
			var hashConfiguration = new XHashConfiguration(deviceConfiguration);
			var configMemoryStream = ZipSerializeHelper.Serialize(hashConfiguration);
			configMemoryStream.Position = 0;
			var configBytes = configMemoryStream.ToArray();
			return SHA256.Create().ComputeHash(configBytes).ToList();
		}
		public static List<byte> CreateHash2(XDeviceConfiguration deviceConfiguration, XDevice gkDevice)
		{
			UpdateConfigurationHelper.Update(deviceConfiguration);
			UpdateConfigurationHelper.PrepareDescriptors(deviceConfiguration);
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
		void InitializeFileBytes(XDeviceConfiguration deviceConfiguration)
		{
			ZipFileConfigurationHelper.SaveToZipFile("configFileToGK", deviceConfiguration);
			var file = File.OpenRead("configFileToGK");
			FileSize = file.Length;
			FileBytes = File.ReadAllBytes(file.Name).ToList();
			file.Close();
		}
		void InitializeInfoBlock()
		{
			InfoBlock = new List<byte>(256) { MinorVersion, MajorVersion };
			InfoBlock.AddRange(Hash1);
			InfoBlock.AddRange(Hash2);
			InfoBlock.AddRange(BitConverter.GetBytes(DescriptorsCount));
			InfoBlock.AddRange(BitConverter.GetBytes(FileSize));
			InfoBlock.AddRange(BitConverter.GetBytes(Date.Ticks));
			while (InfoBlock.Count < 256)
				InfoBlock.Add(0);
		}

		public static GKFileInfo InfoBlockToGKInfo(List<byte> infoBytes)
		{
			return new GKFileInfo
			{
				InfoBlock = infoBytes,
				MinorVersion = infoBytes[0],
				MajorVersion = infoBytes[1],
				Hash1 = infoBytes.GetRange(2, 32),
				Hash2 = infoBytes.GetRange(34, 32),
				DescriptorsCount = BitConverter.ToInt32(infoBytes.GetRange(66, 4).ToArray(), 0),
				FileSize = BitConverter.ToInt64(infoBytes.GetRange(70, 8).ToArray(), 0),
				Date = DateTime.FromBinary(BitConverter.ToInt64(infoBytes.GetRange(78, 8).ToArray(), 0)),
				FileBytes = new List<byte>()
			};
		}
	}
}
