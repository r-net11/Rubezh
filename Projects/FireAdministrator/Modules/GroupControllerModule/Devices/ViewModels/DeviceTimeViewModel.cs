using System;
using System.Collections.Generic;
using Common.GK;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public static class DeviceTimeHelper
	{
		public static string Read(XDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 4, 6);
			if (sendResult.HasError)
			{
				MessageBoxService.Show("Ошибка связи с устройством");
				return "";
			}
			var Day = sendResult.Bytes[0];
			var Month = sendResult.Bytes[1];
			var Year = sendResult.Bytes[2];
			var Hour = sendResult.Bytes[3];
			var Minute = sendResult.Bytes[4];
			var Second = sendResult.Bytes[5];
			var stringTime = Day.ToString() + "/" + Month.ToString() + "/" + Year.ToString() + " " + Hour.ToString() + ":" + Minute.ToString() + ":" + Second.ToString();
			return stringTime;
		}

		public static void Write(XDevice device)
		{
			var dateTime = DateTime.Now;
			var bytes = new List<byte>();
			bytes.Add((byte)dateTime.Day);
			bytes.Add((byte)dateTime.Month);
			bytes.Add((byte)(dateTime.Year - 2000));
			bytes.Add((byte)dateTime.Hour);
			bytes.Add((byte)dateTime.Minute);
			bytes.Add((byte)dateTime.Second);
			SendManager.Send(device, 6, 5, 0, bytes);
		}
	}
}