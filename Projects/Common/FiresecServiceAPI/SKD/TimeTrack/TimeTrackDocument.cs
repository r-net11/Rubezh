using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class TimeTrackDocument
	{
		public TimeTrackDocumentType TimeTrackDocumentType { get; set; }

		public TimeTrackDocument()
		{
			UID = Guid.NewGuid();
			DocumentCode = 1;
			StartDateTime = DateTime.Now;
			EndDateTime = DateTime.Now;
			DocumentDateTime = DateTime.Now;
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
		public int DocumentCode { get; set; }

		[DataMember]
		public string Comment { get; set; }

		[DataMember]
		public DateTime DocumentDateTime { get; set; }

		[DataMember]
		public int DocumentNumber { get; set; }
	}
}