using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Collections;
using System.Diagnostics;
using ServerFS2.Monitor;
using ServerFS2.ConfigurationWriter;
using FS2Api;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
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

		public static void ResetStates(List<PaneleResetItem> panelResetItems)
		{
			foreach (var paneleResetBit in panelResetItems)
			{
				var parentPanel = ConfigurationCash.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == paneleResetBit.PanelUID);
				if (parentPanel == null)
				{
					throw new FS2Exception("Прибор для сброса не найден");
				}
				var statusBytes = GetDeviceStatus(parentPanel);
				var statusBytesArray = new byte[] { statusBytes[3], statusBytes[2], statusBytes[1], statusBytes[0], statusBytes[7], statusBytes[6], statusBytes[5], statusBytes[4] };
				var bitArray = new BitArray(statusBytesArray);
				foreach (var stateId in paneleResetBit.Ids)
				{
					var metadataPanelState = MetadataHelper.Metadata.panelStates.FirstOrDefault(x => x.ID == stateId);
					if (metadataPanelState != null)
					{
						if (metadataPanelState.@class == "0")
						{
							ResetFire(parentPanel);
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
				var bytes = CreateBytesArray(parentPanel.Parent.IntAddress + 2, parentPanel.IntAddress, 0x02, 0x10, newStatusBytes);
				SendCode(bytes);
			}
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

		public static void AddToIgnoreList(Device device)
		{
			SendCodeToPanel(device, 0x02, 0x54, 0x0B, 0x01, 0x00, device.AddressOnShleif, 0x00, 0x00, 0x00, device.ShleifNo - 1);
		}

		public static void RemoveFromIgnoreList(Device device)
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