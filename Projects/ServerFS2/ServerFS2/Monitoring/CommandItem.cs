using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ServerFS2.Monitor
{
	public class CommandItem
	{
		const int expiredTime = 10;
		Device Device;
		string CommandName;
		DateTime StartTme;
		public bool Sended;
		public bool Expired;

		public CommandItem(Device device, string commandName)
		{
			Device = device;
			CommandName = commandName;
			Sended = false;
			Expired = false;
			StartTme = DateTime.Now;
		}

		public void Send()
		{
			ServerHelper.ExecuteCommand(Device, CommandName);
			Sended = true;
		}

		public void CheckForExpired()
		{
			var tableNo = MetadataHelper.GetDeviceTableNo(Device);
			var deviceId = MetadataHelper.GetIdByUid(Device.DriverUID);
			var devicePropInfo = MetadataHelper.Metadata.devicePropInfos.FirstOrDefault(x => (x.tableType == tableNo) && (x.name == CommandName));

			if (DateTime.Now - StartTme > TimeSpan.FromSeconds(expiredTime))
			{
				Expired = true;
				return;
			}

			if (devicePropInfo.off == "0")
				Expired = Device.DeviceState.States.Any(x => x.DriverState.Id == devicePropInfo.expectedState);
			else if (devicePropInfo.off == "1")
				Expired = Device.DeviceState.States.Any(x => x.DriverState.Id != devicePropInfo.expectedState);
		}
	}
}