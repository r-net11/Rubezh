using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using FiresecAPI.SKD;
using Infrastructure.Common;

namespace SKDDriver
{
	public static class SKDSynchronyzation
	{
		public static void AddCard(SKDCard card)
		{
			foreach (var cardZone in card.CardDoors)
			{
				var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == cardZone.DoorUID);
				if (zone != null)
				{
					foreach (var device in SKDManager.Devices)
					{
						if (device.ZoneUID == cardZone.DoorUID)
						{
							WriteCardToReader(device, card, cardZone);
						}
					}
				}
			}
		}

		static void WriteCardToReader(SKDDevice device, SKDCard card, CardDoor cardDoor)
		{
			var bytes = new List<byte>();
			bytes.Add(9);
			bytes.Add((byte)device.IntAddress);
			bytes.Add((byte)(cardDoor.IsAntiPassback ? 1 : 0));
			bytes.Add((byte)(cardDoor.IsComission ? 1 : 0));
			bytes.Add(0); // interval type
			bytes.Add(0); // interval no
			SKDDeviceProcessor.SendBytes(device, bytes);
		}

		static TimeIntervalsConfiguration Intervals
		{
			get { return SKDManager.SKDConfiguration.TimeIntervalsConfiguration; }
		}

		public static bool WriteIntervals(SKDDevice device)
		{
			var bytes = new List<byte>();
			bytes.Add(10);

			var hashBytes = GetTimeIntervalsHash(Intervals);
			bytes.AddRange(hashBytes);

			bytes.Add(1);
			bytes.Add((byte)Intervals.TimeIntervals.Count);
			foreach (var timeInterval in Intervals.TimeIntervals)
			{
				bytes.Add((byte)timeInterval.TimeIntervalParts.Count);
				foreach (var timeIntervalPart in timeInterval.TimeIntervalParts)
				{
					bytes.Add((byte)timeIntervalPart.StartTime.Hour);
					bytes.Add((byte)timeIntervalPart.StartTime.Minute);
					bytes.Add((byte)timeIntervalPart.StartTime.Second);
					bytes.Add((byte)timeIntervalPart.EndTime.Hour);
					bytes.Add((byte)timeIntervalPart.EndTime.Minute);
					bytes.Add((byte)timeIntervalPart.EndTime.Second);
				}
			}

			bytes.Add(2);
			bytes.Add((byte)Intervals.WeeklyIntervals.Count);
			foreach (var weeklyTimeInterval in Intervals.WeeklyIntervals)
			{
				bytes.Add((byte)weeklyTimeInterval.WeeklyIntervalParts.Count);
				foreach (var weeklyIntervalPart in weeklyTimeInterval.WeeklyIntervalParts)
				{
					var timeInterval = Intervals.TimeIntervals.FirstOrDefault(x => x.ID == weeklyIntervalPart.TimeIntervalID);
					if (timeInterval != null)
					{
						bytes.Add((byte)weeklyIntervalPart.No);
						bytes.Add((byte)Intervals.TimeIntervals.IndexOf(timeInterval));
					}
				}
			}

			bytes.Add(3);
			bytes.Add((byte)SKDManager.SKDConfiguration.TimeIntervalsConfiguration.SlideDayIntervals.Count);
			foreach (var slideDayInterval in SKDManager.SKDConfiguration.TimeIntervalsConfiguration.SlideDayIntervals)
			{
				bytes.Add((byte)slideDayInterval.TimeIntervalIDs.Count);
				bytes.Add((byte)(slideDayInterval.StartDate.Year - 2000));
				bytes.Add((byte)slideDayInterval.StartDate.Month);
				bytes.Add((byte)slideDayInterval.StartDate.Day);
				foreach (var timeIntervalUID in slideDayInterval.TimeIntervalIDs)
				{
					var timeInterval = Intervals.TimeIntervals.FirstOrDefault(x => x.ID == timeIntervalUID);
					if (timeInterval != null)
					{
						bytes.Add((byte)Intervals.TimeIntervals.IndexOf(timeInterval));
					}
				}
			}

			bytes.Add(4);
			bytes.Add((byte)Intervals.SlideWeeklyIntervals.Count);
			foreach (var slideWeeklyInterval in Intervals.SlideWeeklyIntervals)
			{
				bytes.Add((byte)slideWeeklyInterval.WeeklyIntervalIDs.Count);
				bytes.Add((byte)(slideWeeklyInterval.StartDate.Year - 2000));
				bytes.Add((byte)slideWeeklyInterval.StartDate.Month);
				bytes.Add((byte)slideWeeklyInterval.StartDate.Day);
				foreach (var weeklyIntervalUID in slideWeeklyInterval.WeeklyIntervalIDs)
				{
					var weeklyInterval = Intervals.WeeklyIntervals.FirstOrDefault(x => x.ID == weeklyIntervalUID);
					if (weeklyInterval != null)
					{
						bytes.Add((byte)Intervals.WeeklyIntervals.IndexOf(weeklyInterval));
					}
				}
			}

			bytes.Add(5);
			bytes.Add((byte)Intervals.Holidays.Count);
			foreach (var holiday in Intervals.Holidays)
			{
				bytes.Add((byte)holiday.TypeNo);
				bytes.Add((byte)(holiday.DateTime.Year - 2000));
				bytes.Add((byte)holiday.DateTime.Month);
				bytes.Add((byte)holiday.DateTime.Day);
			}

			return !SKDDeviceProcessor.SendBytes(device, bytes).HasError;
		}

		public static List<byte> GetTimeIntervalsHash(TimeIntervalsConfiguration timeIntervalsConfiguration)
		{
			var configMemoryStream = ZipSerializeHelper.Serialize(timeIntervalsConfiguration);
			configMemoryStream.Position = 0;
			var configBytes = configMemoryStream.ToArray();
			return SHA256.Create().ComputeHash(configBytes).ToList();
		}
	}
}