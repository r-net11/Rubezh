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
		public Guid PassJournalUID { get; set; }

		[DataMember]
		public TimeSpan StartTime { get; set; }

		[DataMember]
		public TimeSpan EndTime { get; set; }

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
				var result = EndTime - StartTime;
				var isCrossNight = EndTime >= new TimeSpan(23, 59, 00);
				if (isCrossNight)
					result += new TimeSpan(0, 1, 0); //TODO:
				return result;
			}
		}
	}
}