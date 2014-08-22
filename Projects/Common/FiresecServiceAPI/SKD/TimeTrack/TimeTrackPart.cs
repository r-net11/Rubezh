using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class TimeTrackPart
	{
		public TimeTrackPart()
		{
			TimeTrackDocumentTypes = new List<TimeTrackDocumentType>();
		}

		[DataMember]
		public TimeSpan StartTime { get; set; }

		[DataMember]
		public TimeSpan EndTime { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public TimeTrackPartType TimeTrackPartType { get; set; }

		[DataMember]
		public TimeTrackDocumentType MinTimeTrackDocumentType { get; set; }

		[DataMember]
		public List<TimeTrackDocumentType> TimeTrackDocumentTypes { get; set; }

		public TimeSpan Delta
		{
			get { return EndTime - StartTime; }
		}
	}
}