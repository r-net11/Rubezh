using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
		static void ResetFire(Device device)
		{
			USBManager.Send(device, 0x02, 0x54, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
		}

		public static void ResetOnePanelStates(Device panelDevice, IEnumerable<string> stateIds)
		{
			var statusBytes = GetDeviceStatus(panelDevice);
			var statusBytesArray = new byte[] { statusBytes[3], statusBytes[2], statusBytes[1], statusBytes[0], statusBytes[7], statusBytes[6], statusBytes[5], statusBytes[4] };
			var bitArray = new BitArray(statusBytesArray);
			foreach (var stateId in stateIds)
			{
				var metadataPanelState = MetadataHelper.Metadata.panelStates.FirstOrDefault(x => x.ID == stateId);
				if (metadataPanelState != null)
				{
					if (metadataPanelState.@class == "0")
					{
						ResetFire(panelDevice);

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
			USBManager.Send(panelDevice, 0x02, 0x10, newStatusBytes);
		}

		public static List<byte> GetDeviceStatus(Device device)
		{
			var result = new List<byte>();
			var response1 = USBManager.Send(device, 0x01, 0x10);
			var response2 = USBManager.Send(device, 0x01, 0x0F);
			result.AddRange(response1.Bytes);
			result.AddRange(response2.Bytes);
			return result;
		}

		public static List<byte> GetExcessDevicesCount(Device device)
		{
			return USBManager.Send(device, 0x01, 0x13).Bytes;
		}

		public static List<byte> GetDustfilledDevicesCount(Device device)
		{
			return USBManager.Send(device, 0x01, 0x56).Bytes;
		}
		

		public static bool PingDevice(Device device)
		{
			return USBManager.Send(device, 0x3C).Bytes[6] == 0x7C;
		}

		public static void ExecuteCommand(Device device, string commandName)
		{
			var tableNo = MetadataHelper.GetDeviceTableNo(device);
			var deviceId = MetadataHelper.GetIdByUid(device.DriverUID);
			var devicePropInfo = MetadataHelper.Metadata.devicePropInfos.FirstOrDefault(x => (x.tableType == tableNo) && (x.name == commandName));
			USBManager.Send(device.Parent, 0x02, 0x53, Convert.ToByte(devicePropInfo.command1.Substring(1, 2), 16), deviceId, device.AddressOnShleif, device.ShleifNo - 1, Convert.ToByte(devicePropInfo.shiftInMemory.Substring(1, 2), 16), Convert.ToByte(devicePropInfo.maskCmdDev.Substring(1, 2), 16), Convert.ToByte(devicePropInfo.commandDev.Substring(1, 2), 16), device.Driver.DriverType == DriverType.MRO ? 0x01 : 0x00);
		}
	}
}