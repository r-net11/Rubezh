using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class PlannedTimeTrackCollection
	{
		public PlannedTimeTrackCollection()
		{
			PlannedTimeTrackParts = new List<PlannedTimeTrackPart>();
		}

		public PlannedTimeTrackCollection(string error) :
			this()
		{
			Error = error;
		}

		[DataMember]
		public List<PlannedTimeTrackPart> PlannedTimeTrackParts { get; private set; }

		//[DataMember]
		//public bool IsIgnoreHoliday { get; set; }

		//[DataMember]
		//public bool IsOnlyFirstEnter { get; set; }

		//[DataMember]
		//public TimeSpan AllowedLate { get; set; }

		//[DataMember]
		//public TimeSpan AllowedEarlyLeave { get; set; }

		[DataMember]
		public TimeSpan SlideTime { get; set; }

		[DataMember]
		public bool IsHoliday { get; set; }

		[DataMember]
		public int HolidayReduction { get; set; }

		[DataMember]
		public string Error { get; set; }
	}

	[DataContract]
	public class PlannedTimeTrackPart
	{
		public PlannedTimeTrackPart()
		{
			TimeTrackParts = new List<TimeTrackPart>();
		}

		public PlannedTimeTrackPart(string error) :
			this()
		{
			Error = error;
		}

		[DataMember]
		public DateTime DateTime { get; set; }

		[DataMember]
		public List<TimeTrackPart> TimeTrackParts { get; set; }

		[DataMember]
		public bool IsHoliday { get; set; }

		[DataMember]
		public int HolidayReduction { get; set; }

		[DataMember]
		public TimeSpan SlideTime { get; set; }

		[DataMember]
		public string Error { get; set; }
	}
}