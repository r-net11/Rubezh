using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ClientFS2.ConfigurationWriter;
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
        static readonly UsbRunner UsbRunner;
		static int _usbRequestNo;

        static ServerHelper()
        {
            var str = DateConverter.ConvertToBytes(DateTime.Now);
            MetadataHelper.Initialize();
            ConfigurationManager.Load();
            Drivers = ConfigurationManager.DriversConfiguration.Drivers;
            UsbRunner = new UsbRunner();
            UsbRunner.Open();
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

        public static bool IsExtendedMode { get; private set; }
        private static void IsExtendedModeMethod()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
            bytes.Add(0x01);
            bytes.Add(0x01);
            bytes.Add(0x37);
            var res = SendCode(bytes).Result;
            IsExtendedMode = res.FirstOrDefault().Data[6] == 1;
        }

            IsExtendedModeMethod();
        public static List<byte> SendRequest(List<byte> bytes)
        {
            return SendCode(bytes, 100000, 100000).Result.FirstOrDefault().Data;
        }

        public static void SynchronizeTime(Device device)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
            bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
            bytes.Add((byte)device.IntAddress);
            bytes.Add(0x02);
            bytes.Add(0x11);
            bytes.AddRange(DateConverter.ConvertToBytes(DateTime.Now));
            SendCode(bytes);
        }
    }
}