using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FS2Api;

namespace ServerFS2.Operations
{
	public static class GuardUsersOperationHelper
	{
		public static List<GuardUser> DeviceGetGuardUsers(Device device)
		{
			var guardUsers = new List<GuardUser>();
			var result = new List<byte>();
			var packetLenght = USBManager.IsUsbDevice(device) ? 0x33 : 0xFF;
			for (var i = 0x14000; i < 0x14E00; i += packetLenght + 1)
			{
				var response = USBManager.Send(device, "Чтение охранных пользователей", 0x01, 0x52, BitConverter.GetBytes(i).Reverse(), packetLenght);
			    if (response.HasError)
			    	throw new FS2Exception(response.Error);
				if (response.Bytes.Count != packetLenght + 1)
					throw new FS2Exception("Количество байт в ответе не совпадает с запрошенным");
			    result.AddRange(response.Bytes);
			}

			var guardUsersCount = result[0];
			//var guardUsersBytes = result.GetRange(1, 13);
			for (int i = 0; i < guardUsersCount; i++)
			{
				var guardUser = new GuardUser();
				guardUser.CanUnSetZone = Convert.ToBoolean((result[i * 46 + 14])&1);
				guardUser.CanSetZone = Convert.ToBoolean((result[i * 46 + 14] >> 1)&1);
				guardUser.Name = BytesHelper.BytesToStringDescription(result.GetRange(i * 46 + 15, 20));
				guardUser.KeyTM = BytesHelper.BytesToString(result.GetRange(i*46 + 35, 6));
				guardUser.Password = BytesToPasswordString(result.GetRange(i * 46 + 41, 3));
				var guardZonesBytes = result.GetRange(i*46 + 44, 16);
				var localZones = GetLocalZones(device);
				for (int j = 0; j < guardZonesBytes.Count; j++)
				{
					if (localZones.Count <= j * 8)
						break;
					for (int k = 0; k < 8; k++)
					{
						if (localZones.Count <= j * 8 + k)
							break;
						if (((guardZonesBytes[j] >> k) & 1) == 1)
						{
							var guardZone = localZones[j*8 + k];
							if (guardZone != null)
								guardUser.ZoneUIDs.Add(guardZone.UID);
						}
					}
				}
				guardUsers.Add(guardUser);
			}
			return guardUsers;
		}

		public static void DeviceSetGuardUsers(Device device, List<GuardUser> guardUsers)
		{
			for (int i = 0; i < guardUsers.Count; i++)
				guardUsers[i].Id = i + 1;
				SetConfigurationOperationHelper.ConfirmLongTermOperation(device);
			// Данные таблицы
			var guardUsersCount = (byte) guardUsers.Count; // 1 байт
			var guardUsersBytes = GetGuardUsersByte(guardUsers);

			var bytes = USBManager.CreateBytesArray(guardUsersCount, guardUsersBytes);
			foreach (var guardUser in guardUsers)
			{
				bytes.AddRange(GuardUserDataToBytesList(device, guardUser));
			}

			// Добавление пустых пользователей
			var emptyGuardUser = new GuardUser();
			emptyGuardUser.KeyTM = new string('0', 12);
			emptyGuardUser.Password = "";

			for(int i = guardUsers.Count; i < 80; i++)
			{
				bytes.AddRange(GuardUserDataToBytesList(device, emptyGuardUser));
			}

			var begin = 0x14000;
			for (int i = 0; i < bytes.Count; i = i + 0x100)
			{
				USBManager.Send(device, "Запись охранных пользователей", 0x02, 0x52, BitConverter.GetBytes(begin + i).Reverse(), Math.Min(bytes.Count - i - 1, 0xFF), bytes.GetRange(i, Math.Min(bytes.Count - i, 0x100)));
			}
		}

		static List<byte> PasswordStringToBytes(string password)
		{
			var passwordByte = new List<byte>();
			var newPasswordString = new String('F',6);
			for (int i = 0; i < password.Length; i++)
			{
				if (i % 2 == 0)
				{
					newPasswordString = newPasswordString.Remove(i + 1, 1);
					newPasswordString = newPasswordString.Insert(i + 1, password[i].ToString());
				}
				else
				{
					newPasswordString = newPasswordString.Remove(i - 1, 1);
					newPasswordString = newPasswordString.Insert(i - 1, password[i].ToString());
				}
			}
			var newPasswordByte = HexStringToByteArray(newPasswordString);
			return newPasswordByte;
		}

		static string BytesToPasswordString(List<byte> bytes)
		{
			var result = "";
			foreach (var b in bytes)
			{
				var c1 = b & 15;
				var c2 = (b>>4) & 15;
				if (c1 != 15)
					result += c1;
				if (c2 != 15)
					result += c2;
			}
			return result;
		}

		static List<Zone> GetLocalZones(Device device)
		{
			var localZones = ConfigurationManager.Zones.Where(zone => zone.DevicesInZone.FirstOrDefault(x => x.Parent == device) != null).ToList();
			localZones.OrderBy(x => x.No);
			return localZones;
		}

		static List<byte> GetGuardUsersByte(List<GuardUser> guardUsers)
		{
			var guardUsersBytes = new List<byte>(13); // 13 байт

			for (int i = 0; i < 13; i++)
			{
				guardUsersBytes.Add(new byte());
				for (int j = 0; j < 8; j++)
				{
					var guardUser = guardUsers.FirstOrDefault(x => x.Id == (i*8 + j + 1));
					if (guardUser != null)
						guardUsersBytes[i] += (byte) (1 << (guardUser.Id - i*8 - 1));
				}
			}
			return guardUsersBytes;
		}

		static List<byte> GetGuardZones(Device device, GuardUser guardUser)
		{
			var guardZonesBytes = new List<byte>(16);
			var localZones = GetLocalZones(device);
			var guardZones = new List<Zone>();

			foreach (var zoneUID in guardUser.ZoneUIDs)
			{
				if (!guardZones.Any(x => x.UID == zoneUID))
					guardZones.Add(ConfigurationManager.Zones.FirstOrDefault(x => x.UID == zoneUID));
			}

		
			var localNos = new List<int>();
			foreach (var guardZone in guardZones)
			{
				var localNo = localZones.IndexOf(localZones.FirstOrDefault(x => x.UID == guardZone.UID));
				localNos.Add(localNo);
			}

			for (int i = 0; i < 16; i++)
			{
				guardZonesBytes.Add(new byte());
				for (int j = 0; j < 8; j++)
				{
					if (localNos.Any(x => x == (i * 8 + j)))
						guardZonesBytes[i] += (byte)(1 << j);
				}
			}

			return guardZonesBytes;
		}

		static List<byte> GuardUserDataToBytesList(Device device, GuardUser guardUser)
		{
			var bytes = new List<byte>();
			// Даннные пользователей
			var guardUserAttribute = (byte)(Convert.ToByte(guardUser.CanSetZone) * 2 + Convert.ToByte(guardUser.CanUnSetZone));
			var guardUserName = BytesHelper.StringToBytes(guardUser.Name);
			var guardUserKeyTM = HexStringToByteArray(guardUser.KeyTM);
			var guardUserPassword = PasswordStringToBytes(guardUser.Password);
			var guardZonesBytes = GetGuardZones(device, guardUser);

			bytes.AddRange(USBManager.CreateBytesArray(guardUserAttribute, guardUserName,
			guardUserKeyTM, guardUserPassword, guardZonesBytes));

			return bytes;
		}

		public static List<byte> HexStringToByteArray(string hex)
		{
			hex = hex.Replace(" ", "");
			return Enumerable.Range(0, hex.Length)
				 .Where(x => x % 2 == 0)
				 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
				 .ToList();
		}
	}
}