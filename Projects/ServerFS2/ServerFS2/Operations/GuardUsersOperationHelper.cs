using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			byte guardUsersByte = 0;
			byte guardZonesByte = 0;
			var localZones = GetLocalZones(device);
			var guardZones = new List<Zone>();
			foreach (var guardUser in guardUsers)
			{
				guardUsersByte += (byte)Math.Pow(2, guardUser.Id - 1);
				foreach (var zoneUID in guardUser.ZoneUIDs)
				{
					if (guardZones.FirstOrDefault(x => x.UID == zoneUID) == null)
						guardZones.Add(ConfigurationManager.Zones.FirstOrDefault(x => x.UID == zoneUID));
				}
			}
			
			foreach (var guardZone in guardZones)
			{
				var localNo = localZones.IndexOf(localZones.FirstOrDefault(x => x.UID == guardZone.UID));
				guardZonesByte += Convert.ToByte(Math.Pow(2, localNo));
			}
			var bytes = USBManager.CreateBytesArray((byte)guardUsers.Count, guardUsersByte);
			foreach (var guardUser in guardUsers)
			{
				var configByte = Convert.ToInt32(guardUser.CanSetZone) * 2 + Convert.ToInt32(guardUser.CanUnSetZone);
				bytes.AddRange(USBManager.CreateBytesArray(guardUser.KeyTM, configByte, BytesHelper.StringToBytes(guardUser.Name), 
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, CreatePasswordBytes(guardUser.Password), guardZonesByte, 0x00, 0x00, 0x00));
			}
			USBManager.Send(device, bytes);
			//throw new FS2Exception("Функция пока не реализована");
		}

		static List<byte> CreatePasswordBytes(string password)
		{
			var passwordByte = new List<byte>();
			var newPasswordString = "";
			for (int i = 0; i < password.Length; i++)
			{
				if (i % 2 == 0)
					newPasswordString.Insert(i + 1, password[i].ToString());
				else
					newPasswordString.Insert(i - 1, password[i].ToString());
			}
			for (int i = password.Length; i < 6; i++)
			{
				newPasswordString += 'F';
			}
			var newPasswordByte = BytesHelper.StringToBytes(newPasswordString, 6);
			return newPasswordByte;
		}

		static List<Zone> GetLocalZones(Device device)
		{

			var localZones = ConfigurationManager.Zones.Where(zone => zone.DevicesInZone.FirstOrDefault(x => x.Parent == device) != null).ToList();
			localZones.OrderBy(x => x.No);
			return localZones;
		}
	}
}