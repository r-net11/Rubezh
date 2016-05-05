using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
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

	public class PlannedSheduleDayCollection
	{
		public PlannedSheduleDayCollection()
		{
			PlannedSheduleDays = new List<PlannedSheduleDay>();
		}

		public PlannedSheduleDayCollection(string error) :
			this()
		{
			Error = error;
		}

		public List<PlannedSheduleDay> PlannedSheduleDays { get; private set; }

		[DataMember]
		public string Error { get; set; }
	}

	public class PlannedSheduleDay
	{
		public int DayNo { get; set; }
	}
}