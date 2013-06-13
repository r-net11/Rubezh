using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Diagnostics;
using ServerFS2.ConfigurationWriter;

namespace ServerFS2
{
	public static class USBManager
	{
		static int RequestNo = 0;

		public static void Initialize()
		{
			var usbDevices = new List<Device>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.RootDevice.Children)
			{
				switch (device.Driver.DriverType)
				{
					case FiresecAPI.Models.DriverType.MS_1:
					case FiresecAPI.Models.DriverType.MS_2:
						usbDevices.Add(device);
						break;
				}
			}

			var usbRunners = new List<UsbRunner2>();
			var usbRunnerDetectors = new List<UsbRunnerDetector>();

			while (true)
			{
				try
				{
					var usbRunner = new UsbRunner2();
					var result = usbRunner.Open();
					if (!result)
						break;
					usbRunners.Add(usbRunner);
					var usbRunnerDetector = new UsbRunnerDetector()
					{
						UsbRunner = usbRunner
					};
					usbRunnerDetectors.Add(usbRunnerDetector);
				}
				catch (Exception)
				{
					break;
				}
			}

			foreach (var usbRunner in usbRunners)
			{
				var serialNo = GetSerialNo(usbRunner);
			}

			foreach (var device in usbDevices)
			{
				var serialNoProperty = device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
				if (serialNoProperty != null)
				{
					var serialNo = serialNoProperty.Value;
				}
			}
		}

		static string GetSerialNo(UsbRunner2 usbRunner)
		{
			var bytes = new List<byte>();
			bytes.Add(0x01);
			bytes.Add(0x01);
			bytes.Add(0x32);
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var result = usbRunner.AddRequest(RequestNo++, bytesList, 1000, 1000, true);
			var responce = result.Result.FirstOrDefault();
			if (responce != null)
			{
				responce.Data.RemoveRange(0, 6);
				var serialNo = BytesHelper.BytesToStringDescription(responce.Data);
				Trace.WriteLine(serialNo);
				return serialNo;
			}
			return null;
		}
	}

	public class UsbRunnerDetector
	{
		public UsbRunner2 UsbRunner { get; set; }
		public string SerialNo { get; set; }
		public string TypeNo { get; set; }
		public Device Device { get; set; }
	}
}