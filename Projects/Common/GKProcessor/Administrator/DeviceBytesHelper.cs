using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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
			var sendResult = SendManager.Send(device, 0, 16, 0, null, true, false, 4000);
			if (sendResult.HasError)
			{
				MessageBoxService.ShowError("Устройство " + device.PresentationName + " недоступно");
				return false;
			}
			return true;
		}

		public static bool GoToTechnologicalRegime(XDevice device)
		{
			if (IsInTechnologicalRegime(device))
				return true;

			LoadingService.DoStep(device.PresentationName + " Переход в технологический режим");
			SendManager.Send(device, 0, 14, 0, null, device.DriverType == XDriverType.GK);
			for (int i = 0; i < 10; i++)
			{
				if (IsInTechnologicalRegime(device))
					return true;
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}

			return false;
		}

		public static bool IsInTechnologicalRegime(XDevice device)
		{
			var sendResult = SendManager.Send(device, 0, 1, 1);
			if (!sendResult.HasError)
			{
				if (sendResult.Bytes.Count > 0)
				{
					var version = sendResult.Bytes[0];
					if (version > 127)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool EraseDatabase(XDevice device)
		{
			LoadingService.DoStep(device.PresentationName + " Стирание базы данных");
			for (int i = 0; i < 3; i++)
			{
				var sendResult = SendManager.Send(device, 0, 15, 0, null, true, false, 10000);
				if (!sendResult.HasError)
				{
					return true;
				}
				else
				{
					Thread.Sleep(TimeSpan.FromSeconds(1));
				}
			}
			return false;
		}

		public static bool GoToWorkingRegime(XDevice device)
		{
			LoadingService.IsCanceled = false;
			LoadingService.DoStep(device.PresentationName + " Переход в рабочий режим");
			if (LoadingService.IsCanceled)
				return true;
			SendManager.Send(device, 0, 11, 0, null, device.DriverType == XDriverType.GK);

			for (int i = 0; i < 10; i++)
			{
				if (LoadingService.IsCanceled)
					return true;
				var sendResult = SendManager.Send(device, 0, 1, 1);
				if (!sendResult.HasError)
				{
					if (sendResult.Bytes.Count > 0)
					{
						var version = sendResult.Bytes[0];
						if (version <= 127)
						{
							return true;
						}
					}
				}
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}

			return false;
		}

		public static bool Ping(XDevice gkDevice)
		{
			var sendResult = SendManager.Send(gkDevice, 0, 1, 1);
			if (sendResult.HasError)
			{
				return false;
			}
			return true;
		}
	}
}