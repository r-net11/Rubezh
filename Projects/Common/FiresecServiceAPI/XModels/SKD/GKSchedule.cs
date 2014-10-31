using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
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
			DayScheduleUIDs = new List<Guid>();
		}

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
		/// Список составных идентификаторов дневных графиков
		/// </summary>
		[DataMember]
		public List<Guid> DayScheduleUIDs { get; set; }
	}
}