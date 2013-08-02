using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using FiresecAPI.Models;
using FS2Api;

namespace ServerFS2.Operations
{
	public static class GuardUsersOperationHelper
	{
		public static List<GuardUser> DeviceGetGuardUsers(Device device)
		{
			throw new FS2Exception("Функция пока не реализована");
		}

		public static void DeviceSetGuardUsers(Device device, List<GuardUser> guardUsers)
		{
			// Данные таблицы
			var guardUsersCount = (byte) guardUsers.Count; // 1 байт
			var guardUsersBytes = GetGuardUsersByte(guardUsers);

			var bytes = USBManager.CreateBytesArray(guardUsersCount, guardUsersBytes);
			foreach (var guardUser in guardUsers)
			{
				// Даннные пользователей
				var guardUserAttribute = GetUserAttribute(guardUser);
				var guardUserName = BytesHelper.StringToBytes(guardUser.Name);
				var guardUserKeyTM = BytesHelper.HexStringToByteArray(guardUser.KeyTM);
				var guardUserPassword = CreatePasswordBytes(guardUser.Password);
				var guardZonesBytes = GetGuardZones(device, guardUser);

				bytes.AddRange(USBManager.CreateBytesArray(guardUserAttribute, guardUserName,
				guardUserKeyTM, guardUserPassword, guardZonesBytes));
			}
			USBManager.Send(device, bytes);
			//throw new FS2Exception("Функция пока не реализована");
		}

		static List<byte> CreatePasswordBytes(string password)
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
			var newPasswordByte = BytesHelper.HexStringToByteArray(newPasswordString);
			return newPasswordByte;
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
				if (!guardZones.Any())
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
					if (!localNos.Any(x => x == (i * 8 + j)))
						guardZonesBytes[i] += (byte)(1 << j);
				}
			}

			return guardZonesBytes;
		}

		static byte GetUserAttribute(GuardUser guardUser)
		{
			return (byte)(Convert.ToByte(guardUser.CanSetZone) * 2 + Convert.ToByte(guardUser.CanUnSetZone));
		}
	}
}