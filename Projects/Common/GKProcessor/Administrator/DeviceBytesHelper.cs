using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Common;
using FiresecAPI;
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
				return "Устройство недоступно";
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
				if (result1.HasError)
					return null;

				byte softvareVersion = result1.Bytes[0];
				if (softvareVersion > 127)
					stringBuilder.AppendLine("Режим: Технологический");
				else
					stringBuilder.AppendLine("Режим: Рабочий");
				softvareVersion = (byte)(softvareVersion << 1);
				softvareVersion = (byte)(softvareVersion >> 1);
				stringBuilder.AppendLine("Версия ПО: " + softvareVersion.ToString());

				var result2 = SendManager.Send(device, 0, 2, 8);
				if (result2.HasError)
					if (result1.HasError)
						return null;

				var serialNo = (ushort)BytesHelper.SubstructInt(result2.Bytes, 0);
				stringBuilder.AppendLine("Серийный номер: " + serialNo.ToString());

				var hardvareVervion = (ushort)BytesHelper.SubstructInt(result2.Bytes, 4);
				stringBuilder.AppendLine("Аппаратный номер: " + hardvareVervion.ToString());

				return stringBuilder.ToString();
			}
			catch (Exception e)
			{
				Logger.Error(e, "DeviceBytesHelper.ShowInfoCommand");
				return "Внутренняя ошибка при выполнении операции";
			}
		}

		public static bool GoToTechnologicalRegime(XDevice device, GKProgressCallback progressCallback)
		{
			if (IsInTechnologicalRegime(device))
				return true;

			GKProcessorManager.DoProgress(device.PresentationName + " Переход в технологический режим", progressCallback);
			SendManager.Send(device, 0, 14, 0, null, device.DriverType == XDriverType.GK);
			for (int i = 0; i < 10; i++)
			{
				if (progressCallback.IsCanceled)
					return false;
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

		public static bool EraseDatabase(XDevice device, GKProgressCallback progressCallback)
		{
			GKProcessorManager.DoProgress(device.PresentationName + " Стирание базы данных", progressCallback);
			for (int i = 0; i < 3; i++)
			{
				if (progressCallback.IsCanceled)
					return false;
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

		public static bool GoToWorkingRegime(XDevice device, GKProgressCallback progressCallback)
		{
			progressCallback.IsCanceled = false;
			GKProcessorManager.DoProgress(device.PresentationName + " Переход в рабочий режим", progressCallback);
			if (progressCallback.IsCanceled)
				return true;
			SendManager.Send(device, 0, 11, 0, null, device.DriverType == XDriverType.GK);

			for (int i = 0; i < 10; i++)
			{
				if (progressCallback.IsCanceled)
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