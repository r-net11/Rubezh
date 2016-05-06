using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	public class TimeIntervalsConfiguration : VersionedConfiguration
	{
		public static readonly string PredefinedIntervalNameNever = "<Никогда>";
		public static readonly string PredefinedIntervalNameAlways = "<Всегда>";
		public static readonly string PredefinedIntervalNameCard = "<Карта>";
		public static readonly string PredefinedIntervalNamePassword = "<Пароль замка>";
		public static readonly string PredefinedIntervalNameCardAndPassword = "<Карта и пароль>";

		/// <summary>
		/// Конструктор по умолчанию
		/// </summary>
		public TimeIntervalsConfiguration()
		{
			DayIntervals = new List<SKDDayInterval>();
			WeeklyIntervals = new List<SKDWeeklyInterval>();
			SlideDayIntervals = new List<SKDSlideDayInterval>();
			SlideWeeklyIntervals = new List<SKDSlideWeeklyInterval>();
			Holidays = new List<SKDHoliday>();
			DoorDayIntervals = new List<SKDDoorDayInterval>();
			DoorWeeklyIntervals = new List<SKDDoorWeeklyInterval>();
		}

		[DataMember]
		public List<SKDDayInterval> DayIntervals { get; set; }

		[DataMember]
		public List<SKDWeeklyInterval> WeeklyIntervals { get; set; }

		[DataMember]
		public List<SKDSlideDayInterval> SlideDayIntervals { get; set; }

		[DataMember]
		public List<SKDSlideWeeklyInterval> SlideWeeklyIntervals { get; set; }

		[DataMember]
		public List<SKDHoliday> Holidays { get; set; }

		[DataMember]
		public List<SKDDoorDayInterval> DoorDayIntervals { get; set; }

		[DataMember]
		public List<SKDDoorWeeklyInterval> DoorWeeklyIntervals { get; set; }

		public bool Validate()
		{
			return ValidateHolidays()
				& ValidateIntervals()
				& ValidateSlideIntervals()
				& ValidateDoorIntervals();
		}

		/// <summary>
		/// Создает праздничные дни по умолчанию
		/// </summary>
		private void CreatePredefinedHolidays()
		{
			Holidays.Add(new SKDHoliday() { Name = "Новогодние каникулы", DateTime = new DateTime(2000, 1, 1) });
			Holidays.Add(new SKDHoliday() { Name = "Новогодние каникулы", DateTime = new DateTime(2000, 1, 2) });
			Holidays.Add(new SKDHoliday() { Name = "Новогодние каникулы", DateTime = new DateTime(2000, 1, 3) });
			Holidays.Add(new SKDHoliday() { Name = "Новогодние каникулы", DateTime = new DateTime(2000, 1, 4) });
			Holidays.Add(new SKDHoliday() { Name = "Рождество", DateTime = new DateTime(2000, 1, 7) });
			Holidays.Add(new SKDHoliday()
			{
				Name = "День советской армии и военно-морского флота",
				DateTime = new DateTime(2000, 2, 23)
			});
			Holidays.Add(new SKDHoliday() { Name = "Международный женский день", DateTime = new DateTime(2000, 3, 8) });
			Holidays.Add(new SKDHoliday() { Name = "День победы", DateTime = new DateTime(2000, 5, 9) });
			Holidays.Add(new SKDHoliday() { Name = "День России", DateTime = new DateTime(2000, 6, 12) });
			Holidays.Add(new SKDHoliday() { Name = "День примерения", DateTime = new DateTime(2000, 11, 4) });
			Holidays.Add(new SKDHoliday() { Name = "Новый год", DateTime = new DateTime(2000, 12, 31) });
		}

		/// <summary>
		/// Создает дневные графики доступа по умолчанию
		/// </summary>
		private void CreatePredefinedDayIntervals()
		{
			// Дневной график доступа <Никогда>
			var neverDayInterval = new SKDDayInterval { Name = PredefinedIntervalNameNever };
			DayIntervals.Add(neverDayInterval);

			// Дневной график доступа <Всегда>
			var alwaysDayInterval = new SKDDayInterval { Name = PredefinedIntervalNameAlways };
			alwaysDayInterval.DayIntervalParts.Add(new SKDDayIntervalPart() { StartMilliseconds = 0, EndMilliseconds = new TimeSpan(23, 59, 59).TotalMilliseconds });
			DayIntervals.Add(alwaysDayInterval);
		}

		/// <summary>
		/// Создает недельные графики доступа по умолчанию
		/// </summary>
		private void CreatePredefinedWeeklyIntervals()
		{
			// Недельный график доступа <Никогда>
			var neverWeeklyInterval = new SKDWeeklyInterval(true) { Name = PredefinedIntervalNameNever, ID = 0 };
			var neverDayInterval = DayIntervals.FirstOrDefault(x => x.Name == PredefinedIntervalNameNever);
			if (neverDayInterval != null)
				foreach (var weeklyIntervalPart in neverWeeklyInterval.WeeklyIntervalParts)
				{
					weeklyIntervalPart.DayIntervalUID = neverDayInterval.UID;
				}
			WeeklyIntervals.Add(neverWeeklyInterval);

			// Недельный график доступа <Всегда>
			var alwaysWeeklyInterval = new SKDWeeklyInterval(true) { Name = PredefinedIntervalNameAlways, ID = 1 };
			var alwaysDayInterval = DayIntervals.FirstOrDefault(x => x.Name == PredefinedIntervalNameAlways);
			if (alwaysDayInterval != null)
				foreach (var weeklyIntervalPart in alwaysWeeklyInterval.WeeklyIntervalParts)
				{
					weeklyIntervalPart.DayIntervalUID = alwaysDayInterval.UID;
				}
			WeeklyIntervals.Add(alwaysWeeklyInterval);
		}

		/// <summary>
		/// Создает дневные графики замка по умолчанию
		/// </summary>
		private void CreatePredefinedDoorDayIntervals()
		{
			var dictionary = new Dictionary<string, SKDDoorConfiguration_DoorOpenMethod>
			{
				{PredefinedIntervalNameCard, SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD},
				{PredefinedIntervalNamePassword, SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_PWD_ONLY},
				{PredefinedIntervalNameCardAndPassword, SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD_FIRST}
			};

			foreach (var dictionaryItem in dictionary)
			{
				var predefinedDayInterval = DoorDayIntervals.FirstOrDefault(x => x.Name == dictionaryItem.Key);
				if (predefinedDayInterval != null)
					continue;
				predefinedDayInterval = new SKDDoorDayInterval { Name = dictionaryItem.Key };
				predefinedDayInterval.DayIntervalParts.Add(new SKDDoorDayIntervalPart()
				{
					StartMilliseconds = 0,
					EndMilliseconds = new TimeSpan(23, 59, 59).TotalMilliseconds,
					DoorOpenMethod = dictionaryItem.Value
				});
				DoorDayIntervals.Add(predefinedDayInterval);
			}
		}

		/// <summary>
		/// Создает недельные графики замка по умолчанию
		/// </summary>
		private void CreatePredefinedDoorWeeklyIntervals()
		{
			var dictionary = new Dictionary<string, SKDDoorDayInterval>()
			{
				{PredefinedIntervalNameCard, DoorDayIntervals.FirstOrDefault(x => x.Name == PredefinedIntervalNameCard)},
				{PredefinedIntervalNamePassword, DoorDayIntervals.FirstOrDefault(x => x.Name == PredefinedIntervalNamePassword)},
				{PredefinedIntervalNameCardAndPassword, DoorDayIntervals.FirstOrDefault(x => x.Name == PredefinedIntervalNameCardAndPassword)}
			};

			var i = 0;
			foreach (var item in dictionary)
			{
				var weeklyInterval = new SKDDoorWeeklyInterval(true) { Name = item.Key, ID = i++ };
				if (item.Value != null)
					foreach (var weeklyIntervalPart in weeklyInterval.WeeklyIntervalParts)
					{
						weeklyIntervalPart.DayIntervalUID = item.Value.UID;
					}
				DoorWeeklyIntervals.Add(weeklyInterval);
			}
		}

		/// <summary>
		/// Проверяет корректность заданных праздников
		/// </summary>
		/// <returns>true - праздники уже в конфигурации,
		/// false - праздники были созданы, т.к. отсутствовали в конфигурации</returns>
		private bool ValidateHolidays()
		{
			if (Holidays.Count != 0)
				return true;
			CreatePredefinedHolidays();
			return false;
		}

		private bool ValidateIntervals()
		{
			var result = true;

			// Проверка наличия предопределенных дневных графиков доступа
			if (DayIntervals.Count == 0)
			{
				CreatePredefinedDayIntervals();
				result = false;
			}

			// Проверка наличия предопределенных недельных графиков доступа
			if (WeeklyIntervals.Count == 0)
			{
				CreatePredefinedWeeklyIntervals();
				result = false;
			}

			// Количество недельных графиков доступа не должно превышать 128 шт.
			if (WeeklyIntervals.RemoveAll(x => x.ID > 127) > 0)
				result = false;

			WeeklyIntervals = WeeklyIntervals.GroupBy(item => item.ID).Select(group => group.First()).ToList();

			foreach (var weeklyInterval in WeeklyIntervals)
				if (weeklyInterval.WeeklyIntervalParts == null)
				{
					weeklyInterval.WeeklyIntervalParts = SKDWeeklyInterval.CreateParts();
					result = false;
				}

			return result;
		}

		private bool ValidateSlideIntervals()
		{
			var result = true;

			if (SlideWeeklyIntervals.RemoveAll(x => x.ID > 127) > 0)
				result = false;
			if (SlideDayIntervals.RemoveAll(x => x.ID > 127) > 0)
				result = false;

			SlideWeeklyIntervals = SlideWeeklyIntervals.GroupBy(item => item.ID).Select(group => group.First()).ToList();
			SlideDayIntervals = SlideDayIntervals.GroupBy(item => item.ID).Select(group => group.First()).ToList();

			foreach (var slideWeeklyInterval in SlideWeeklyIntervals)
				if (slideWeeklyInterval.WeeklyIntervalIDs.RemoveAll(id => id < 0 || id > 128) > 0)
					result = false;
			foreach (var slideDayInterval in SlideDayIntervals)
				if (slideDayInterval.DayIntervalIDs.RemoveAll(id => id < 0 || id > 128) > 0)
					result = false;

			return result;
		}

		private bool ValidateDoorIntervals()
		{
			var result = true;

			// Проверка наличия предопределенных дневных графиков замка
			if (DoorDayIntervals.Count == 0)
			{
				CreatePredefinedDoorDayIntervals();
				result = false;
			}

			// Проверка наличия предопределенных недельных графиков замка
			if (DoorWeeklyIntervals.Count == 0)
			{
				CreatePredefinedDoorWeeklyIntervals();
				result = false;
			}

			// Количество недельных графиков замков не должно превышать 128 шт.
			if (DoorWeeklyIntervals.RemoveAll(x => x.ID > 127) > 0)
				result = false;

			DoorWeeklyIntervals = DoorWeeklyIntervals.GroupBy(item => item.ID).Select(group => group.First()).ToList();

			foreach (var weeklyInterval in DoorWeeklyIntervals)
				if (weeklyInterval.WeeklyIntervalParts == null)
				{
					weeklyInterval.WeeklyIntervalParts = SKDDoorWeeklyInterval.CreateParts();
					result = false;
				}

			return result;
		}
	}
}