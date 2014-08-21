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
			DocumentCodes = new List<int>();
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
		public int DocumentCode { get; set; }

		[DataMember]
		public List<int> DocumentCodes { get; set; }

		public TimeSpan Delta
		{
			get { return EndTime - StartTime; }
		}
	}
}