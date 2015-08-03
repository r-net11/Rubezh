using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public static class GKScheduleHelper
	{
		/// <summary>
		/// Перезаписать вае графики конкретного ГК
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		public static OperationResult<bool> RewriteAllSchedules(GKDevice device)
		{
			var progressCallback = GKProcessorManager.StartProgress("Перезапись графиков в " + device.PresentationName, "Стирание графиков", 1, false, GKProgressClientType.Administrator);
			var removeResult = RemoveAllSchedules(device);
			if (removeResult.HasError)
				return OperationResult<bool>.FromError(removeResult.Errors);

			var schedules = new List<GKSchedule>();
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				var schedulesResult = databaseService.GKScheduleTranslator.Get();
				if (schedulesResult.HasError)
					return OperationResult<bool>.FromError(schedulesResult.Errors);
				schedules = schedulesResult.Result;
			}

			progressCallback = GKProcessorManager.StartProgress("Запись графиков в " + device.PresentationName, "", schedules.Count + 1, false, GKProgressClientType.Administrator);
			var emptySchedule = new GKSchedule();
			emptySchedule.Name = "Никогда";
			var setResult = GKSetSchedule(device, emptySchedule);
			if (setResult.HasError)
				return OperationResult<bool>.FromError(setResult.Errors);
			GKProcessorManager.DoProgress("Запись пустого графика ", progressCallback);
			int i = 1;
			foreach (var schedule in schedules)
			{
				setResult = GKSetSchedule(device, schedule);
				if (setResult.HasError)
					return OperationResult<bool>.FromError(setResult.Errors);
				GKProcessorManager.DoProgress("Запись графика " + i, progressCallback);
				i++;
			}
			GKProcessorManager.StopProgress(progressCallback);
			return new OperationResult<bool>(true);
		}

		/// <summary>
		/// Стереть все графики конкретного ГК
		/// </summary>
		/// <param name="device"></param>
		/// <returns></returns>
		static OperationResult<bool> RemoveAllSchedules(GKDevice device)
		{
			for (int no = 1; no <= 255; no++)
			{
				var bytes = new List<byte>();
				bytes.Add(0);
				bytes.Add((byte)no);
				var nameBytes = BytesHelper.StringDescriptionToBytes("");
				bytes.AddRange(nameBytes);
				for (int i = 0; i < 255 - 32; i++)
				{
					bytes.Add(0);
				}

				var sendResult = SendManager.Send(device, (ushort)(bytes.Count), 28, 0, bytes);
				if (sendResult.HasError)
					return OperationResult<bool>.FromError(sendResult.Error, true);
			}
			return new OperationResult<bool>(true);
		}

		/// <summary>
		/// Записать график в конкретный ГК
		/// </summary>
		/// <param name="device"></param>
		/// <param name="schedule"></param>
		/// <returns></returns>
		static OperationResult<bool> GKSetSchedule(GKDevice device, GKSchedule schedule)
		{
			var count = 0;
			var daySchedules = new List<GKDaySchedule>();
			using (var databaseService = new SKDDriver.DataClasses.DbService())
			{
				var schedulesResult = databaseService.GKDayScheduleTranslator.Get();
				if (schedulesResult.HasError)
					return OperationResult<bool>.FromError(schedulesResult.Errors);
				daySchedules = schedulesResult.Result;
			}
			if (schedule.ScheduleType == GKScheduleType.Access)
			{
				foreach (var schedulePart in schedule.ScheduleParts)
				{
					var daySchedule = daySchedules.FirstOrDefault(x => x.UID == schedulePart.DayScheduleUID);
					if (daySchedule != null)
					{
						count += daySchedule.DayScheduleParts.Count;
					}
				}
			}
			else
			{
				count = schedule.Calendar.SelectedDays.Count;
			}

			int secondsPeriod = 0;
			switch(schedule.ScheduleType)
			{
				case GKScheduleType.Access:
					switch(schedule.SchedulePeriodType)
					{
						case GKSchedulePeriodType.Weekly:
						case GKSchedulePeriodType.Dayly:
							secondsPeriod = schedule.ScheduleParts.Count * 60 * 60 * 24;
							break;
						case GKSchedulePeriodType.Custom:
							secondsPeriod = schedule.HoursPeriod * 60 * 60;
							break;
						case GKSchedulePeriodType.NonPeriodic:
							secondsPeriod = 0;
							break;
					}
					break;

				case GKScheduleType.Holiday:
				case GKScheduleType.WorkHoliday:
					secondsPeriod = 0;
					break;
			}

			var bytes = new List<byte>();
			bytes.Add((byte)schedule.No);
			var nameBytes = BytesHelper.StringDescriptionToBytes(schedule.Name);
			bytes.AddRange(nameBytes);
			bytes.Add((byte)schedule.HolidayScheduleNo);
			bytes.AddRange(BytesHelper.ShortToBytes((ushort)(count * 2)));
			bytes.AddRange(BytesHelper.IntToBytes(secondsPeriod));
			bytes.Add((byte)schedule.WorkHolidayScheduleNo);
			bytes.Add(0);
			bytes.Add(0);
			bytes.Add(0);
			bytes.Add(0);
			bytes.Add(0);
			bytes.Add(0);
			bytes.Add(0);

			var startDateTime = schedule.StartDateTime;
			if (schedule.ScheduleType == GKScheduleType.Access && schedule.SchedulePeriodType == GKSchedulePeriodType.Weekly)
			{
				if (startDateTime.DayOfWeek == DayOfWeek.Monday)
					startDateTime = startDateTime.AddDays(0);
				if (startDateTime.DayOfWeek == DayOfWeek.Tuesday)
					startDateTime = startDateTime.AddDays(-1);
				if (startDateTime.DayOfWeek == DayOfWeek.Wednesday)
					startDateTime = startDateTime.AddDays(-2);
				if (startDateTime.DayOfWeek == DayOfWeek.Thursday)
					startDateTime = startDateTime.AddDays(-3);
				if (startDateTime.DayOfWeek == DayOfWeek.Friday)
					startDateTime = startDateTime.AddDays(-4);
				if (startDateTime.DayOfWeek == DayOfWeek.Saturday)
					startDateTime = startDateTime.AddDays(-5);
				if (startDateTime.DayOfWeek == DayOfWeek.Sunday)
					startDateTime = startDateTime.AddDays(-6);
			}
			var timeSpan = new DateTime(startDateTime.Year, startDateTime.Month, startDateTime.Day) - new DateTime(2000, 1, 1);
			var scheduleStartSeconds = timeSpan.TotalSeconds;

			if (schedule.ScheduleType == GKScheduleType.Access)
			{
				foreach (var schedulePart in schedule.ScheduleParts.OrderBy(x => x.DayNo))
				{
					var daySchedule = daySchedules.FirstOrDefault(x => x.UID == schedulePart.DayScheduleUID);
					if (daySchedule != null)
					{
						foreach (var daySchedulePart in daySchedule.DayScheduleParts.OrderBy(x => x.StartMilliseconds))
						{
							bytes.AddRange(BytesHelper.IntToBytes((int)(scheduleStartSeconds + 24 * 60 * 60 * schedulePart.DayNo + daySchedulePart.StartMilliseconds / 1000)));
							bytes.AddRange(BytesHelper.IntToBytes((int)(scheduleStartSeconds + 24 * 60 * 60 * schedulePart.DayNo + daySchedulePart.EndMilliseconds / 1000)));
						}
					}
				}
			}
			else
			{
				foreach (var day in schedule.Calendar.SelectedDays.OrderBy(x => x.Date))
				{
					bytes.AddRange(BytesHelper.IntToBytes((int)((day - new DateTime(2000, 1, 1)).TotalSeconds)));
					bytes.AddRange(BytesHelper.IntToBytes((int)((day - new DateTime(2000, 1, 1) + TimeSpan.FromDays(1)).TotalSeconds)));
				}
			}

			var packs = new List<List<byte>>();
			for (int packNo = 0; packNo <= bytes.Count / 256; packNo++)
			{
				int packLenght = Math.Min(256, bytes.Count - packNo * 256);
				var packBytes = bytes.Skip(packNo * 256).Take(packLenght).ToList();

				if (packBytes.Count > 0)
				{
					var resultBytes = new List<byte>();
					resultBytes.Add((byte)(packNo));
					resultBytes.AddRange(packBytes);
					packs.Add(resultBytes);
				}
			}

			foreach (var pack in packs)
			{
				var sendResult = SendManager.Send(device, (ushort)(pack.Count), 28, 0, pack);
				if (sendResult.HasError)
				{
					return OperationResult<bool>.FromError(sendResult.Error);
				}
			}

			return new OperationResult<bool>(true);
		}

		/// <summary>
		/// Записать один график во все ГК
		/// </summary>
		/// <param name="schedule"></param>
		/// <returns></returns>
		public static OperationResult SetSchedule(GKSchedule schedule)
		{
			try
			{
				foreach (var device in GKManager.Devices.Where(x => x.DriverType == GKDriverType.GK))
				{
					var result = GKSetSchedule(device, schedule);
					if (result.HasError)
						return new OperationResult(result.Error);
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		/// <summary>
		/// Записать несколько графиков во все ГК
		/// </summary>
		/// <param name="daySchedule"></param>
		/// <returns>true всегда</returns>
		public static OperationResult<bool> SetSchedules(IEnumerable<GKSchedule> schedules)
		{
			try
			{
				var operationResult = new OperationResult<bool>(true);
				foreach (var device in GKManager.Devices.Where(x => x.DriverType == GKDriverType.GK))
				{
					foreach (var schedule in schedules)
					{
						var result = GKSetSchedule(device, schedule);
						if (result.HasError)
						{
							operationResult.Errors.AddRange(result.Errors);
							break;
						}
					}
				}
				return operationResult;
			}
			catch (Exception e)
			{
				return OperationResult<bool>.FromError(e.Message, true);
			}
		}

		/// <summary>
		/// Стереть график из всех ГК
		/// </summary>
		/// <param name="schedule"></param>
		/// <returns></returns>
		public static OperationResult RemoveSchedule(int scheduleNo)
		{
			var schedule = new GKSchedule();
			schedule.No = scheduleNo;
			return SetSchedule(schedule);
		}
	}
}