using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using ServerFS2.Helpers;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
		public static event Action<int, int, string> Progress;
		public static List<Driver> Drivers;
		static readonly object Locker = new object();
		public static readonly UsbRunner UsbRunner;
		static int _usbRequestNo;
		static ServerHelper()
		{
			var str = DateConverter.ConvertToBytes(DateTime.Now);
			MetadataHelper.Initialize();
			ConfigurationManager.Load();
			Drivers = ConfigurationManager.DriversConfiguration.Drivers;
			UsbRunner = new UsbRunner();
			try
			{
				UsbRunner.Open();
			}
			catch { }
		}
		public static void Initialize()
		{
			;
		}
		public static OperationResult<List<Response>> SendCode(List<List<byte>> bytesList, int maxDelay = 1000, int maxTimeout = 1000, bool IsWrite = false)
		{
			return UsbRunner.AddRequest(bytesList, maxDelay, maxTimeout, IsWrite);
		}
		public static OperationResult<List<Response>> SendCode(List<byte> bytes, int maxDelay = 1000, int maxTimeout = 1000, bool IsWrite = false)
		{
			return UsbRunner.AddRequest(new List<List<byte>> { bytes }, maxDelay, maxTimeout, IsWrite);
		}
		public static bool IsExtendedMode { get; set; }
		public static void SendCodeAsync(List<byte> bytes, int maxDelay = 1000, int maxTimeout = 1000, bool IsWrite = false)
		{
			UsbRunner.AddAsyncRequest(new List<List<byte>> { bytes }, maxDelay, maxTimeout, IsWrite);
		}
		public static void IsExtendedModeMethod()
		{
			//var bytes = CreateBytesArray(BitConverter.GetBytes(++_usbRequestNo).Reverse(), 0x01, 0x01, 0x37);
			//var res = SendCode(bytes).Result;
			//IsExtendedMode = res.FirstOrDefault().Data[6] == 1;
			IsExtendedMode = true;
		}
	    public static bool IsUsbDevice
	    {
            get { return UsbRunner.IsUsbDevice; }
            set 
            { 
                UsbRunner.IsUsbDevice = value;
                UsbRunner.Close();
                UsbRunner.Open();
            }
	    }
		public static List<byte> SendRequest(List<byte> bytes)
		{
			return SendCode(bytes, 100000, 100000).Result.FirstOrDefault().Data;
		}
		public static void SynchronizeTime(Device device)
		{
			var bytes = CreateBytesArray(BitConverter.GetBytes(++_usbRequestNo).Reverse(), Convert.ToByte(device.Parent.IntAddress + 2),
			device.IntAddress, 0x02, 0x11, DateConverter.ConvertToBytes(DateTime.Now));
			SendCode(bytes);
		}
		static List<byte> CreateBytesArray(params object[] values)
		{
			var bytes = new List<byte>();
			foreach (var value in values)
			{
				if (value as IEnumerable<Byte> != null)
					bytes.AddRange((IEnumerable<Byte>)value);
				else
					bytes.Add(Convert.ToByte(value));
			}
			return bytes;
		}
	}
}