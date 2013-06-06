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

namespace ServerFS2
{
    public static partial class ServerHelper
    {
        public static event Action<int, int, string> Progress;
        public static List<Driver> Drivers;
        static readonly object Locker = new object();
        public static readonly UsbRunner UsbRunner;
        static int UsbRequestNo;
        public static bool IsExtendedMode { get; set; }
        static ServerHelper()
        {
            MetadataHelper.Initialize();
            Drivers = ConfigurationManager.DriversConfiguration.Drivers;
            UsbRunner = new UsbRunner();
            try
            {
                UsbRunner.Open();
            }
            catch
            { }
        }

        public static OperationResult<List<Response>> SendCode(List<List<byte>> bytesList, int maxDelay = 1000, int maxTimeout = 1000)
        {
            return UsbRunner.AddRequest(++UsbRequestNo, bytesList, maxDelay, maxTimeout, true);
        }
        
        public static OperationResult<List<Response>> SendCode(List<byte> bytes, int maxDelay = 1000, int maxTimeout = 1000)
        {
            return UsbRunner.AddRequest(++UsbRequestNo, new List<List<byte>> { bytes }, maxDelay, maxTimeout, true);
        }

        public static OperationResult<List<Response>> SendCodeWithoutRequestNo(List<byte> bytes, int maxDelay = 1000, int maxTimeout = 1000)
        {
            return UsbRunner.AddRequest(-1, new List<List<byte>> { bytes }, maxDelay, maxTimeout, true);
        }

        public static OperationResult<List<Response>> SendCodeWithoutRequestNo(List<List<byte>> bytesList, int maxDelay = 1000, int maxTimeout = 1000)
        {
            return UsbRunner.AddRequest(-1, bytesList, maxDelay, maxTimeout, true);
        }

        public static void SendCodeAsync(int usbRequestNo, List<byte> bytes, int maxDelay = 1000, int maxTimeout = 1000)
        {
            UsbRunner.AddRequest(usbRequestNo, new List<List<byte>> { bytes }, maxDelay, maxTimeout, false);
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
            var bytes = CreateBytesArray(Convert.ToByte(device.Parent.IntAddress + 2),
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

        public static void ResetFire(Device device) // 02 54 10 00 00 00 00 00 00 00
        {
            SendCode(CreateBytesArray(device.Parent.IntAddress + 2, device.IntAddress, 0x02, 0x54, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00));
        }

        public static void ResetTest(Device device, List<byte> status)
        {
            status[1] = (byte)(status[1] & ~2);
            var bytes = CreateBytesArray(device.Parent.IntAddress + 2, device.IntAddress, 0x02, 0x10, status.GetRange(0, 4));
            SendCode(bytes);
        }

		public static void ResetPanelBit(Device device, List<byte> statusBytes, int bitNo)
		{
			Trace.WriteLine("ResetPanelBit statusBytes = " + BytesHelper.BytesToString(statusBytes));

			var statusBytesArray = new byte[] { statusBytes[3], statusBytes[2], statusBytes[1], statusBytes[0], statusBytes[7], statusBytes[6], statusBytes[5], statusBytes[4] };
			var bitArray = new BitArray(statusBytesArray);
			bitArray[bitNo] = false;
			var value = 0;
			for (int i = 0; i < bitArray.Count; i++)
			{
				if (bitArray[i])
					value += 1 << i;
			}

			Trace.WriteLine("ResetPanelBit statusValue = " + value);
			var newStatusBytes = BitConverter.GetBytes(value);
			var bytes = CreateBytesArray(device.Parent.IntAddress + 2, device.IntAddress, 0x02, 0x10, newStatusBytes);
			SendCode(bytes);
		}

        public static List<byte> GetDeviceStatus(Device device)
        {
			//if (!PingDevice(device))
			//    return null;
            var bytes1 = CreateBytesArray(device.Parent.IntAddress + 2, device.IntAddress, 0x01, 0x10);
            var bytes2 = CreateBytesArray(device.Parent.IntAddress + 2, device.IntAddress, 0x01, 0x0F);
            List<byte> response1;
            List<byte> response2;
            var result = new List<byte>();
            response1 = SendCode(bytes1).Result.FirstOrDefault().Data;
            response2 = SendCode(bytes2).Result.FirstOrDefault().Data;
            response1.RemoveRange(0, 7);
            response2.RemoveRange(0, 7);
            result.AddRange(response1);
            result.AddRange(response2);
            return result;
        }

        public static void AddDeviceToCheckList(Device device)
        {
            var bytes = CreateBytesArray(device.Parent.Parent.IntAddress + 2, device.Parent.IntAddress, 0x02, 0x54, 0x0B, 0x01, 0x00, device.AddressOnShleif, 0x00, 0x00, 0x00, device.ShleifNo - 1);
            SendCode(bytes);
        }

        public static void RemoveDeviceFromCheckList(Device device)
        {
            var bytes = CreateBytesArray(device.Parent.Parent.IntAddress + 2, device.Parent.IntAddress, 0x02, 0x54, 0x0B, 0x00, 0x00, device.AddressOnShleif, 0x00, 0x00, 0x00, device.ShleifNo - 1);
            SendCode(bytes);
        }

        public static bool PingDevice(Device device)
        {
            var bytes = CreateBytesArray(device.Parent.IntAddress + 2, device.IntAddress, 0x3C);
            return SendCode(bytes).Result.FirstOrDefault().Data[6] == 0x7C;
        }
    }
}