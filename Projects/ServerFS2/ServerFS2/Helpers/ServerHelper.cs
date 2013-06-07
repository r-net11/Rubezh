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
			UsbRunnerBase = new UsbRunner();
			//UsbRunnerBase = new UsbRunner2();
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
			var result = UsbRunner.AddRequest(++UsbRequestNo, new List<List<byte>> { bytes }, 1000, 1000, true).Result[0].Data;
			result.RemoveRange(0, IsUsbDevice ? 2 : 7);
			return result;
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

        public static void ResetFire(Device device)
        {
        	var bytes = CreateBytesArray(0x02, 0x54, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
            SendCodeToPanel(bytes, device);
        }

        public static void ResetTest(Device device, List<byte> status)
        {
            status[1] = (byte)(status[1] & ~2);
			SendCodeToPanel(device, 0x02, 0x10, status.GetRange(0, 4));
			StatesHelper.ChangeDeviceStates(device, device.DeviceState.States);
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
			MonitoringProcessor.DoMonitoring = false;
			SendCode(bytes);
			device.DeviceState.OnStateChanged();
			MonitoringProcessor.DoMonitoring = true;
		}

		public static void ResetStates(List<ResetItem> resetItems)
		{
			var paneleResetBits = new List<PaneleResetBit>();
			foreach (var resetItem in resetItems)
			{
				var parentPanel = resetItem.DeviceState.Device;
				var paneleResetBit = paneleResetBits.FirstOrDefault(x => x.ParentPanel == parentPanel);
				if (paneleResetBit == null)
				{
					paneleResetBit = new PaneleResetBit()
					{
						ParentPanel = parentPanel
					};
					paneleResetBits.Add(paneleResetBit);
				}
				foreach (var deviceDriverState in resetItem.States)
				{
					paneleResetBit.Ids.Add(deviceDriverState.DriverState.Code);
				}
			}

			foreach (var paneleResetBit in paneleResetBits)
			{
				var statusBytes = GetDeviceStatus(paneleResetBit.ParentPanel);
				var statusBytesArray = new byte[] { statusBytes[3], statusBytes[2], statusBytes[1], statusBytes[0], statusBytes[7], statusBytes[6], statusBytes[5], statusBytes[4] };
				var bitArray = new BitArray(statusBytesArray);
				foreach (var stateId in paneleResetBit.Ids)
				{
					var metadataPanelState = MetadataHelper.Metadata.panelStates.FirstOrDefault(x => x.ID == stateId);
					if (metadataPanelState != null)
					{
						if (metadataPanelState.@class == "0")
						{
							ResetFire(paneleResetBit.ParentPanel);
						}
						else
						{
							var bitNo = Int16.Parse(metadataPanelState.no);
							bitArray[bitNo] = false;
						}
					}
				}
				var value = 0;
				for (int i = 0; i < bitArray.Count; i++)
				{
					if (bitArray[i])
						value += 1 << i;
				}

				var newStatusBytes = BitConverter.GetBytes(value);
				var bytes = CreateBytesArray(paneleResetBit.ParentPanel.Parent.IntAddress + 2, paneleResetBit.ParentPanel.IntAddress, 0x02, 0x10, newStatusBytes);
				SendCode(bytes);
			}
		}

		public class PaneleResetBit
		{
			public PaneleResetBit()
			{
				Ids = new HashSet<string>();
			}

			public Device ParentPanel { get; set; }
			public HashSet<string> Ids { get; set; }
		}

        public static List<byte> GetDeviceStatus(Device device)
        {
			//if (!PingDevice(device))
			//    return null;
        	var result = new List<byte>();
			var response1 = SendCodeToPanel(device, 0x01, 0x10);
			var response2 = SendCodeToPanel(device, 0x01, 0x0F);
            result.AddRange(response1);
            result.AddRange(response2);
            return result;
        }

        public static void AddDeviceToCheckList(Device device)
        {
			SendCodeToPanel(device, 0x02, 0x54, 0x0B, 0x01, 0x00, device.AddressOnShleif, 0x00, 0x00, 0x00, device.ShleifNo - 1);
        }

        public static void RemoveDeviceFromCheckList(Device device)
        {
			SendCodeToPanel(device, 0x02, 0x54, 0x0B, 0x00, 0x00, device.AddressOnShleif, 0x00, 0x00, 0x00, device.ShleifNo - 1);
        }

        public static bool PingDevice(Device device)
        {
            var bytes = CreateBytesArray(device.Parent.IntAddress + 2, device.IntAddress, 0x3C);
            return SendCode(bytes)[6] == 0x7C;
        }

		public static void ExecuteCommand(Device device, string commandName)
		{
			var tableNo = MetadataHelper.GetDeviceTableNo(device);
			var deviceId = MetadataHelper.GetIdByUid(device.DriverUID);
			var devicePropInfo = MetadataHelper.Metadata.devicePropInfos.FirstOrDefault(x => (x.tableType == tableNo) && (x.name == commandName));
			SendCodeToPanel(device.Parent, 0x02, 0x53, Convert.ToByte(devicePropInfo.command1.Substring(1, 2), 16), deviceId, device.AddressOnShleif, device.ShleifNo - 1, Convert.ToByte(devicePropInfo.shiftInMemory.Substring(1, 2), 16), Convert.ToByte(devicePropInfo.maskCmdDev.Substring(1, 2), 16), Convert.ToByte(devicePropInfo.commandDev.Substring(1, 2), 16), device.Driver.DriverType == DriverType.MRO ? 0x01 : 0x00);
		}
    }
}