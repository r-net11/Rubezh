using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.Models;
using ServerFS2.Helpers;
using Device = FiresecAPI.Models.Device;
using System.Collections;
using System.Diagnostics;
using ServerFS2.ConfigurationWriter;
using ServerFS2.Service;
using FS2Api;
using ServerFS2.Monitor;

namespace ServerFS2
{
    public static partial class ServerHelper
    {
        public static event Action<int, int, string> Progress;
        public static List<Driver> Drivers;
        static readonly object Locker = new object();
		public static readonly UsbRunnerBase UsbRunnerBase;
        static int UsbRequestNo;
        public static bool IsExtendedMode { get; set; }
        static ServerHelper()
        {
            Drivers = ConfigurationManager.DriversConfiguration.Drivers;
			//UsbRunnerBase = new UsbRunner();
			UsbRunnerBase = new UsbRunner2();
            try
            {
				UsbRunnerBase.Open();
            }
            catch
            { }
        }

		public static List<byte> SendCodeToPanel(List<byte> bytes, Device device, int maxDelay = 1000, int maxTimeout = 1000)
		{
			bytes.InsertRange(0,IsUsbDevice? new List<byte> {(byte) (0x02)}: new List<byte> {(byte) (device.Parent.IntAddress + 2), (byte) device.IntAddress});
			var result = UsbRunnerBase.AddRequest(++UsbRequestNo, new List<List<byte>> { bytes }, maxDelay, maxTimeout, true).Result[0].Data;
			result.RemoveRange(0, IsUsbDevice ? 2 : 7);
			return result;
		}

		public static List<byte> SendCodeToPanel(Device device, params object[] value)
		{
			var bytes = CreateBytesArray(value);
			bytes.InsertRange(0, IsUsbDevice ? new List<byte> { (byte)(0x02) } : new List<byte> { (byte)(device.Parent.IntAddress + 2), (byte)device.IntAddress });
			var result = UsbRunnerBase.AddRequest(++UsbRequestNo, new List<List<byte>> { bytes }, 1000, 1000, true);
			if (result != null)
			{
				var responce = result.Result.FirstOrDefault();
				if (responce != null)
				{
					var data = responce.Data;
					data.RemoveRange(0, IsUsbDevice ? 2 : 7);
					return data;
				}
			}
			return null;
		}

        public static OperationResult<List<Response>> SendCode(List<List<byte>> bytesList, int maxDelay = 1000, int maxTimeout = 1000)
        {
			return UsbRunnerBase.AddRequest(++UsbRequestNo, bytesList, maxDelay, maxTimeout, true);
        }

		public static List<byte> SendCode(List<byte> bytes, int maxDelay = 1000, int maxTimeout = 1000)
        {
			return UsbRunnerBase.AddRequest(++UsbRequestNo, new List<List<byte>> { bytes }, maxDelay, maxTimeout, true).Result[0].Data;
        }
		
        public static void SendCodeAsync(int usbRequestNo, List<byte> bytes, int maxDelay = 1000, int maxTimeout = 1000)
        {
			UsbRunnerBase.AddRequest(usbRequestNo, new List<List<byte>> { bytes }, maxDelay, maxTimeout, false);
        }

        public static bool IsUsbDevice
        {
            get { return UsbRunner.IsUsbDevice; }
            set
            {
                UsbRunner.IsUsbDevice = value;
				UsbRunnerBase.Close();
				UsbRunnerBase.Open();
            }
        }

        public static List<byte> SendRequest(List<byte> bytes)
        {
            return SendCode(bytes);
        }

        public static void SynchronizeTime(Device device)
        {
			SendCodeToPanel(device, 0x02, 0x11, DateConverter.ConvertToBytes(DateTime.Now));
        }

        public static List<byte> CreateBytesArray(params object[] values)
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