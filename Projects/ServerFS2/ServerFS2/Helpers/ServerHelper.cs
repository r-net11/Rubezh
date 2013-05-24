using System;
using System.Collections.Generic;
using System.IO;
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
		public static bool IsExtendedMode { get; set; }

		static ServerHelper()
		{
			var str = DateConverter.ConvertToBytes(DateTime.Now);
			MetadataHelper.Initialize();
			ConfigurationManager.Load();
			Drivers = ConfigurationManager.DriversConfiguration.Drivers;
			UsbRunner = new UsbRunner();
			try
			{
				var result = UsbRunner.Open();
			}
			catch { }
		}

		public static OperationResult<List<Response>> SendCode(List<List<byte>> bytesList, int maxDelay = 1000, int maxTimeout = 1000)
		{
			return UsbRunner.AddRequest(bytesList, maxDelay, maxTimeout, true);
		}

        public static OperationResult<List<Response>> SendCode(List<byte> bytes, int maxDelay = 1000, int maxTimeout = 1000)
		{
			return UsbRunner.AddRequest(new List<List<byte>> { bytes }, maxDelay, maxTimeout, true);
		}

		public static void SendCodeAsync(List<byte> bytes, int maxDelay = 1000, int maxTimeout = 1000)
		{
			UsbRunner.AddRequest(new List<List<byte>> { bytes }, maxDelay, maxTimeout, false);
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

        public static void BytesToFile(string fileName, List<byte> bytes)
        {
            var deviceRamTxt = new StreamWriter("..\\" + fileName);
            int j = 0;
            foreach (var b in bytes)
            {
                deviceRamTxt.Write("{0} ", b.ToString("X2"));
                j++;
                if (j % 16 == 0)
                    deviceRamTxt.Write("\n");
                if (j % 64 == 0)
                    deviceRamTxt.Write("\n");
            }
            deviceRamTxt.Close();
        }
	}
}