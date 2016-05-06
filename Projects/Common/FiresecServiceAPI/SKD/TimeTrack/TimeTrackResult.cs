using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
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
			Documents = new List<TimeTrackDocument>();
		}

		public TimeTrackEmployeeResult(string error) :
			this()
		{
			Error = error;
		}

		[DataMember]
		public ShortEmployee ShortEmployee { get; set; }

		[DataMember]
		public string ScheduleName { get; set; }

		[DataMember]
		public List<DayTimeTrack> DayTimeTracks { get; set; }

		[DataMember]
		public List<TimeTrackDocument> Documents { get; set; }

		[DataMember]
		public string Error { get; set; }
	}
}
