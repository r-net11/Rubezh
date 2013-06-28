using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ServerFS2.Operations
{
	public static class GetInformationOperationHelper
	{
		public static string GetDeviceInformation(Device device)
		{
			string serialNo;
			string softVersion;
			string bdVersion;
			if (device.Properties.FirstOrDefault(x => x.Name == "SerialNo") == null)
				device.Properties.Add(new Property() { Name = "SerialNo", Value = "не определена" });
			if (device.Driver.DriverType == DriverType.MS_1 || device.Driver.DriverType == DriverType.MS_2)
			{
				var serialNoBytes = USBManager.Send(device, 0x01, 0x32).Bytes;
				serialNo = new string(Encoding.Default.GetChars(serialNoBytes.ToArray()));
				device.Properties.FirstOrDefault(x => x.Name == "SerialNo").Value = serialNo;
				var driverTypeBytes = USBManager.Send(device, 0x01, 0x04).MsFlag;

				if (device.Properties.FirstOrDefault(x => x.Name == "DriverType") == null)
					device.Properties.Add(new Property() { Name = "DriverType", Value = "не определен" });
				if (driverTypeBytes == 0x41)
					device.Properties.FirstOrDefault(x => x.Name == "DriverType").Value = FiresecAPI.Models.DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverType == DriverType.MS_2).Name;
				else
					device.Properties.FirstOrDefault(x => x.Name == "DriverType").Value = FiresecAPI.Models.DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverType == DriverType.MS_1).Name;
			}
			else
			{
				var serialNoBytes = USBManager.Send(device, 0x01, 0x52, 0x00, 0x00, 0x00, 0xF4, 0x0B).Bytes;
				serialNo = new string(Encoding.Default.GetChars(serialNoBytes.ToArray()));
				device.Properties.FirstOrDefault(x => x.Name == "SerialNo").Value = serialNo;

				var panelDatabaseReader = new ReadPanelDatabaseOperationHelper(device, false);
				var bdVersionBytes = ServerHelper.GetBytesFromRomDB(device, panelDatabaseReader.GetRomFirstIndex(device) + 4, 2);
				bdVersion = bdVersionBytes[0].ToString("X") + "." + bdVersionBytes[1].ToString("X");

				if (device.Properties.FirstOrDefault(x => x.Name == "BDVersion") == null)
					device.Properties.Add(new Property() { Name = "BDVersion", Value = "не определена" });
				device.Properties.FirstOrDefault(x => x.Name == "BDVersion").Value = bdVersion;
				
				var driverTypeBytes = USBManager.Send(device, 0x01, 0x03).Bytes;
				if (device.Properties.FirstOrDefault(x => x.Name == "DriverType") == null)
					device.Properties.Add(new Property() { Name = "DriverType", Value = "не определен" });
				device.Properties.FirstOrDefault(x => x.Name == "DriverType").Value = DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverCode == driverTypeBytes[0]).Name;
			}
			var softVersionBytes = USBManager.Send(device, 0x01, 0x12).Bytes;
			softVersion = softVersionBytes[0].ToString("X") + "." + softVersionBytes[1].ToString("X");

			if (device.Properties.FirstOrDefault(x => x.Name == "SoftVersion") == null)
				device.Properties.Add(new Property() { Name = "SoftVersion", Value = "не определена" });
			device.Properties.FirstOrDefault(x => x.Name == "SoftVersion").Value = softVersion;

			return "";
		}
	}
}
