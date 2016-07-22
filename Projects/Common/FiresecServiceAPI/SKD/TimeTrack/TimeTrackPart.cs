using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class TimeTrackPart
	{
		public TimeTrackPart()
		{
			TimeTrackDocumentTypes = new List<TimeTrackDocumentType>();
			PassJournalUID = Guid.Empty;
		}

		[DataMember]
		public bool IsManuallyAdded { get; set; }

		/// <summary>
		/// Флаг, отображающий лежит ли интервал за пределами графика работ (касается только неявок по документу)
		/// </summary>
		[DataMember]
		public bool IsOutside { get; set; }

		/// <summary>
		/// Показывает, сделан ли проход в зоне УРВ или нет
		/// </summary>
		[DataMember]
		public bool IsForURVZone { get; set; }

		[DataMember]
		public bool IsNeedAdjustment { get; set; }

		[DataMember]
		public DateTime? AdjustmentDate { get; set; }

		[DataMember]
		public Guid? CorrectedByUID { get; set; }

		[DataMember]
		public bool IsOpen { get; set; }

		[DataMember]
		public bool IsForceClosed { get; set; }

		[DataMember]
		public bool NotTakeInCalculations { get; set; }

		[DataMember]
		public Guid PassJournalUID { get; set; }

		[DataMember]
		public DateTime EnterDateTime { get; set; }

		[DataMember]
		public DateTime? ExitDateTime { get; set; }

		[DataMember]
		public TimeTrackType TimeTrackPartType { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public TimeTrackDocumentType MinTimeTrackDocumentType { get; set; }

		[DataMember]
		public List<TimeTrackDocumentType> TimeTrackDocumentTypes { get; set; }

		[DataMember]
		public bool StartsInPreviousDay { get; set; }

		[DataMember]
		public bool EndsInNextDay { get; set; }

		[DataMember]
		public string DayName { get; set; }

		[DataMember]
		public string Tooltip { get; set; }

		public TimeSpan Delta
		{
			get
			{
				if (!ExitDateTime.HasValue) return TimeSpan.Zero;

				var result = ExitDateTime.Value.TimeOfDay - EnterDateTime.TimeOfDay;
				var isCrossNight = ExitDateTime.Value.TimeOfDay >= new TimeSpan(23, 59, 59) && ExitDateTime.GetValueOrDefault().TimeOfDay != EnterDateTime.TimeOfDay;
				if (isCrossNight)
					result += new TimeSpan(0, 0, 1); //TODO:
				return result;
			}
		}
	}
}