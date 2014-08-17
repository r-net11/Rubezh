using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class TimeTrackResult
	{
		public TimeTrackResult()
		{
			TimeTrackEmployeeResults = new List<TimeTrackEmployeeResult>();
		}

		public TimeTrackResult(string error) :
			this()
		{
			Error = error;
		}

		[DataMember]
		public List<TimeTrackEmployeeResult> TimeTrackEmployeeResults { get; private set; }

		[DataMember]
		public string Error { get; set; }
	}

	[DataContract]
	public class TimeTrackEmployeeResult
	{
		public TimeTrackEmployeeResult()
		{
			DayTimeTracks = new List<DayTimeTrack>();
		}

		public TimeTrackEmployeeResult(string error) :
			this()
		{
			DayTimeTracks = new List<DayTimeTrack>();
			Error = error;
		}

		[DataMember]
		public List<DayTimeTrack> DayTimeTracks { get; set; }

		[DataMember]
		public ShortEmployee ShortEmployee { get; set; }

		[DataMember]
		public bool IsIgnoreHoliday { get; set; }

		[DataMember]
		public bool IsOnlyFirstEnter { get; set; }

		[DataMember]
		public TimeSpan AllowedLate { get; set; }

		[DataMember]
		public TimeSpan AllowedEarlyLeave { get; set; }

		[DataMember]
		public string Error { get; set; }
	}
}