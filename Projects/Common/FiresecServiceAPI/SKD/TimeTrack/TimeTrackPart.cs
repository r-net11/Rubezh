using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
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

		[DataMember]
		public bool IsNeedAdjustment { get; set; }

		[DataMember]
		public DateTime? AdjustmentDate { get; set; }

		[DataMember]
		public Guid? CorrectedByUID { get; set; }

		/// <summary>
		/// Флаг - показывает открытый интервал или нет
		/// <example>EnterDateTime = 03.03.2015, ExitDateTime = null</example>
		/// </summary>
		[DataMember]
		public bool IsOpen { get; set; }

		/// <summary>
		/// Флаг - показывает, когда открытый интервал принудительно закрыт
		/// </summary>
		[DataMember]
		public bool IsForceClosed { get; set; }

		[DataMember]
		public DateTime? EnterTimeOriginal { get; set; }

		[DataMember]
		public DateTime? ExitTimeOriginal { get; set; }

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
				var isCrossNight = ExitDateTime.Value.TimeOfDay >= new TimeSpan(23, 59, 59);
				if (isCrossNight)
					result += new TimeSpan(0, 0, 1); //TODO:
				return result;
			}
		}
	}
}