using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// График работ
	/// </summary>
	[DataContract]
	public class GKSchedule : ModelBase
	{
		public GKSchedule()
		{
			ScheduleType = GKScheduleType.Access;
			SchedulePeriodType = GKSchedulePeriodType.Weekly;
			StartDateTime = DateTime.Now;
			ScheduleParts = new List<GKSchedulePart>();
			Year = DateTime.Now.Year;
			SelectedDays = new List<DateTime>();
		}

		[DataMember]
		public int Year { get; set; }

		[DataMember]
		public List<DateTime> SelectedDays { get; set; }

		/// <summary>
		/// Тип графика
		/// </summary>
		[DataMember]
		public GKScheduleType ScheduleType { get; set; }

		/// <summary>
		/// Тип периодичности графика
		/// </summary>
		[DataMember]
		public GKSchedulePeriodType SchedulePeriodType { get; set; }

		/// <summary>
		/// Начало действия графика
		/// </summary>
		[DataMember]
		public DateTime StartDateTime { get; set; }

		/// <summary>
		/// Период в часах
		/// </summary>
		[DataMember]
		public int HoursPeriod { get; set; }

		/// <summary>
		/// Номер праздничного графика
		/// </summary>
		[DataMember]
		public int HolidayScheduleNo { get; set; }

		/// <summary>
		/// Номер рабочего выходного
		/// </summary>
		[DataMember]
		public int WorkHolidayScheduleNo { get; set; }

		/// <summary>
		/// Составные части графика работ
		/// </summary>
		[DataMember]
		public List<GKSchedulePart> ScheduleParts { get; set; }
	}
}