using System;
using System.Collections.Generic;
using System.Text;
using Common;
using Infrastructure.Common.Windows;
using XFiresecAPI;

namespace GKProcessor
{
	public static class DeviceBytesHelper
	{
		public static string ReadDateTime(XDevice device)
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

		public static bool WriteDateTime(XDevice device)
		{
			var dateTime = DateTime.Now;
			var bytes = new List<byte>();
			bytes.Add((byte)dateTime.Day);
			bytes.Add((byte)dateTime.Month);
			bytes.Add((byte)(dateTime.Year - 2000));
			bytes.Add((byte)dateTime.Hour);
			bytes.Add((byte)dateTime.Minute);
			bytes.Add((byte)dateTime.Second);
			var sendResult = SendManager.Send(device, 6, 5, 0, bytes);
			return !sendResult.HasError;
		}

		public static string GetDeviceInfo(XDevice device)
		{
			try
			{
				var stringBuilder = new StringBuilder();
				var result1 = SendManager.Send(device, 0, 1, 1);
				if (!result1.HasError)
				{
					byte softvareVersion = result1.Bytes[0];
					if (softvareVersion > 127)
						stringBuilder.AppendLine("Режим: Технологический");
					else
						stringBuilder.AppendLine("Режим: Рабочий");
					softvareVersion = (byte)(softvareVersion << 1);
					softvareVersion = (byte)(softvareVersion >> 1);
					stringBuilder.AppendLine("Версия ПО: " + softvareVersion.ToString());
				}
				var result2 = SendManager.Send(device, 0, 2, 8);
				if (!result2.HasError)
				{
					var serialNo = (ushort)BytesHelper.SubstructInt(result2.Bytes, 0);
					stringBuilder.AppendLine("Серийный номер: " + serialNo.ToString());

					var hardvareVervion = (ushort)BytesHelper.SubstructInt(result2.Bytes, 4);
					stringBuilder.AppendLine("Аппаратный номер: " + hardvareVervion.ToString());
				}
				return stringBuilder.ToString();
			}
			catch (Exception e)
			{
				Logger.Error(e, "DeviceBytesHelper.ShowInfoCommand");
			}
			return null;
		}

		public static bool Clear(XDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 16, 0);
			if (sendResult.HasError)
			{
				MessageBoxService.ShowError("Устройство " + device.PresentationDriverAndAddress + " недоступно");
				return false;
			}
			return true;
		}
	}
}