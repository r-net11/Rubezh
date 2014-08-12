using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class TimeTrackException
	{
		public TimeTrackException()
		{
			UID = Guid.NewGuid();
			TimeTrackExceptionType = TimeTrackExceptionType.None;
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid EmployeeUID { get; set; }

		[DataMember]
		public DateTime StartDateTime { get; set; }

		[DataMember]
		public DateTime EndDateTime { get; set; }

		[DataMember]
		public TimeTrackExceptionType TimeTrackExceptionType { get; set; }

		[DataMember]
		public string Comment { get; set; }
	}
}