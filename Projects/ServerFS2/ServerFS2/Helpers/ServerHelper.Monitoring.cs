using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Common;
using System.Text;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
		static void ResetFire(Device device)
		{
			USBManager.Send(device, "Сброс пожара", 0x02, 0x54, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
		}

		static void ResetAlarm(Device device)
		{
			USBManager.Send(device, "Сброс тревоги", 0x02, 0x54, 0x09, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00);
		}

		public static void ResetOnePanelStates(Device panelDevice, IEnumerable<string> stateIds)
		{
			var hasBytesToReset = false;
			var statusBytes = GetDeviceStatus(panelDevice);
			var statusBytesArray = new byte[] { statusBytes[3], statusBytes[2], statusBytes[1], statusBytes[0], statusBytes[7], statusBytes[6], statusBytes[5], statusBytes[4] };
			var bitArray = new BitArray(statusBytesArray);
			foreach (var stateId in stateIds)
			{
				var metadataPanelState = MetadataHelper.Metadata.panelStates.FirstOrDefault(x => x.ID == stateId);
				if (metadataPanelState != null)
				{
					if (metadataPanelState.ID == "Warning")
					{
						ResetFire(panelDevice);
					}
					else
					{
						if (metadataPanelState.@class == "0")
						{
							if (metadataPanelState.ID == "Alarm")
							{
								ResetAlarm(panelDevice);
							}
							else
							{
								ResetFire(panelDevice);
							}
						}
						else
						{
							var bitNo = Int16.Parse(metadataPanelState.no);
							if (bitArray[bitNo] == true)
							{
								bitArray[bitNo] = false;
								hasBytesToReset = true;
							}
						}
					}
				}
			}

			var newStatusBytes = BytesHelper.BytesFromBitArray(bitArray);
			if (hasBytesToReset)
			{
				USBManager.Send(panelDevice, "Установка байт статуса прибора", 0x02, 0x10, newStatusBytes);
			}
		}

		public static List<byte> GetDeviceStatus(Device device)
		{
			var result = new List<byte>();
			var response1 = USBManager.Send(device, "Запрос старших 4 байт статуса прибора", 0x01, 0x10);
			var response2 = USBManager.Send(device, "Запрос младших 4 байт статуса прибора", 0x01, 0x0F);
			result.AddRange(response1.Bytes);
			result.AddRange(response2.Bytes);
			return result;
		}

		public static string GetDeviceInformation(Device device)
		{
			var response = USBManager.Send(device, "Уточнить у Ромы", 0x01, 0x13);
			string serialNo = "";
			List<byte> serialNoBytes;
			if (device.Driver.DriverType == DriverType.MS_1 || device.Driver.DriverType == DriverType.MS_2)
			{
				serialNoBytes = USBManager.Send(device, "Запрос серийного номера МС", 0x01, 0x32).Bytes;
				serialNo = new string(Encoding.Default.GetChars(serialNoBytes.ToArray()));
			}
			else
			{
				response = USBManager.Send(device, "Запрос серийного номера прибора", 0x01, 0x60);
				serialNoBytes = USBManager.Send(device, "Уточнить у Ромы", 0x01, 0x52, 0x00, 0x00, 0x00, 0xF4, 0x0B).Bytes;
				serialNo = new string(Encoding.Default.GetChars(serialNoBytes.ToArray()));
			}
			return serialNo;
		}

		public static bool PingDevice(Device device)
		{
			return USBManager.Send(device, "Пинг", 0x3C).Bytes[6] == 0x7C;
		}

		public static void ExecuteCommand(Device device, string commandName)
		{
			try
			{
				var tableNo = MetadataHelper.GetDeviceTableNo(device);
				if (tableNo != null)
				{
					var deviceId = MetadataHelper.GetIdByUid(device.DriverUID);
					var devicePropInfo = MetadataHelper.Metadata.devicePropInfos.FirstOrDefault(x => (x.tableType == tableNo) && (x.name == commandName));
					if (devicePropInfo != null)
					{
						USBManager.SendShortAttempt(device.Parent, "Выполнение команды", 0x02, 0x53, Convert.ToByte(devicePropInfo.command1.Substring(1, 2), 16), deviceId, device.AddressOnShleif, 0x00, Convert.ToByte(devicePropInfo.shiftInMemory.Substring(1, 2), 16), Convert.ToByte(devicePropInfo.maskCmdDev.Substring(1, 2), 16), Convert.ToByte(devicePropInfo.commandDev.Substring(1, 2), 16), device.ShleifNo - 1);
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error("ServerHelper.ExecuteCommand");
			}
		}
	}
}