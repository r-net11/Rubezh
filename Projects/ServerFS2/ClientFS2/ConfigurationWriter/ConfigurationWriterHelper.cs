using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public static class ConfigurationWriterHelper
	{
		static List<DevicesGroup> DevicesGroups = new List<DevicesGroup>();
		static byte[] bytes1 = new byte[2000];
		static byte[] bytes2 = new byte[2000];
		static List<byte> list2 = new List<byte>();

		public static void Run()
		{
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsPanel)
				{
					FormDeviceDatabase(device);
				}
			}
		}

		static void FormDeviceDatabase(Device panelDevice)
		{
			bytes1[0] = (byte)'b';
			bytes1[1] = (byte)'a';
			bytes1[2] = (byte)'s';
			bytes1[3] = (byte)'e';
			bytes1[4] = 0;
			bytes1[5] = 0;

			for (int i = 0; i < 100; i++)
			{
				list2.Add(0);
			}
			list2.Add(0);
			list2.Add(0);

			var RM_1_Group = CreateDevicesGroup(panelDevice, DriverType.RM_1);
			var MPT_Group = CreateDevicesGroup(panelDevice, DriverType.MPT);
			var MDU_Group = CreateDevicesGroup(panelDevice, DriverType.MDU);
			var Valve_Group = CreateDevicesGroup(panelDevice, DriverType.Valve);

			var SmokeDetector_Group = CreateDevicesGroup(panelDevice, DriverType.SmokeDetector);
			var HeatDetector_Group = CreateDevicesGroup(panelDevice, DriverType.HeatDetector);
			var CombinedDetector_Group = CreateDevicesGroup(panelDevice, DriverType.CombinedDetector);
			var AM_1_Group = CreateDevicesGroup(panelDevice, DriverType.AM_1,
				DriverType.ShuzOffButton, DriverType.ShuzOnButton, DriverType.ShuzUnblockButton, DriverType.StartButton, DriverType.StopButton);
			var HandDetector_Group = CreateDevicesGroup(panelDevice, DriverType.HandDetector);
			var AM1_O_Group = CreateDevicesGroup(panelDevice, DriverType.AM1_O);
			var AMP_4_Group = CreateDevicesGroup(panelDevice, DriverType.AMP_4);
			var AM1_T_Group = CreateDevicesGroup(panelDevice, DriverType.AM1_T);
			var AMT_4_Group = CreateDevicesGroup(panelDevice, DriverType.AMT_4);

			CreateBytesForDetectorGroup(SmokeDetector_Group, 24);
			CreateBytesForDetectorGroup(HeatDetector_Group, 30);
			CreateBytesForDetectorGroup(CombinedDetector_Group, 36);
			CreateBytesForDetectorGroup(AM_1_Group, 42);
			CreateBytesForDetectorGroup(HandDetector_Group, 48);
			CreateBytesForDetectorGroup(AM1_O_Group, 54);
			CreateBytesForDetectorGroup(AMP_4_Group, 78);
			CreateBytesForDetectorGroup(AM1_T_Group, 96);
			CreateBytesForDetectorGroup(AMT_4_Group, 102);
		}

		static void CreateBytesForDetectorGroup(DevicesGroup devicesGroup, int offset)
		{
			devicesGroup.Offset = list2.Count;
			devicesGroup.Count = devicesGroup.Devices.Count;
			foreach (var device in devicesGroup.Devices)
			{
				var address = device.IntAddress % 256;
				var shleif = device.IntAddress / 256 - 1;
				byte internalParameter1 = 0;
				byte internalParameter2 = 0;
				byte dynamicParameter = 0;
				short zoneNo = 0;
				if (device.Zone != null)
				{
					zoneNo = (short)device.Zone.No;
				}

				var deviceBytes = new List<byte>();
				deviceBytes.Add((byte)address);
				deviceBytes.Add((byte)shleif);
				deviceBytes.Add(internalParameter1);
				deviceBytes.Add(internalParameter2);
				deviceBytes.Add(dynamicParameter);
				deviceBytes.AddRange(BytesHelper.ShortToBytes(zoneNo));

				var smokeParameterValue = 18;
				var smokeParameter = device.Properties.FirstOrDefault(x => x.Name == "AU_Smoke");
				if (smokeParameter != null)
				{
					smokeParameterValue = Int32.Parse(smokeParameter.Value);
				}

				var temperatureParameterValue = 18;
				var temperatureParameter = device.Properties.FirstOrDefault(x => x.Name == "AU_Temperature");
				if (temperatureParameter != null)
				{
					temperatureParameterValue = Int32.Parse(temperatureParameter.Value);
				}

				byte computerConfigurationData = 0;

				var dataBytes = new List<byte>();
				switch (device.Driver.DriverType)
				{
					case DriverType.SmokeDetector:
						dataBytes.Add((byte)smokeParameterValue);
						break;
					case DriverType.HeatDetector:
						dataBytes.Add((byte)temperatureParameterValue);
						break;
					case DriverType.CombinedDetector:
						dataBytes.Add((byte)computerConfigurationData);
						dataBytes.Add((byte)smokeParameterValue);
						dataBytes.Add((byte)temperatureParameterValue);
						break;
				}

				deviceBytes.Add((byte)dataBytes.Count);
				deviceBytes.AddRange(dataBytes);
				list2.AddRange(deviceBytes);
			}

			var offsetBytes = BitConverter.GetBytes(devicesGroup.Offset);
			bytes1[offset + 0] = offsetBytes[1];
			bytes1[offset + 1] = offsetBytes[2];
			bytes1[offset + 2] = offsetBytes[3];
			bytes1[offset + 3] = (byte)devicesGroup.Lenght;
			bytes1[offset + 4] = (byte)devicesGroup.Count;
		}

		static DevicesGroup CreateDevicesGroup(Device panelDevice, params DriverType[] driverTypes)
		{
			var devicesGroup = new DevicesGroup(driverTypes[0]);
			foreach (var device in panelDevice.Children)
			{
				if (driverTypes.Contains(device.Driver.DriverType))
				{
					devicesGroup.Devices.Add(device);
				}
			}
			DevicesGroups.Add(devicesGroup);
			return devicesGroup;
		}
	}

	class DevicesGroup
	{
		public DevicesGroup(DriverType driverType)
		{
			DriverType = driverType;
			Devices = new List<Device>();
		}

		public DriverType DriverType { get; set; }
		public List<Device> Devices { get; set; }
		public int Offset { get; set; }
		public int Lenght { get; set; }
		public int Count { get; set; }
	}
}