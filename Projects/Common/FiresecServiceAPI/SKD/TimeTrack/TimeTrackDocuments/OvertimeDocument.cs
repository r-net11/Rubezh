using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class OvertimeDocument : ITimeTrackDocument
	{
		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public bool IsOutside { get; set; }

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

		[DataMember]
		public TimeTrackDocumentType TimeTrackDocumentType { get; set; }

		[DataMember]
		public string FileName { get; set; }

		public OvertimeDocument()
		{
			TimeTrackDocumentType = new TimeTrackDocumentType
			{
				DocumentType = DocumentType.Overtime
			};
		}

		public OvertimeDocument(string name, string shortName, int code)
		{
			TimeTrackDocumentType = new TimeTrackDocumentType
			{
				DocumentType = DocumentType.Overtime,
				Code = code,
				ShortName = shortName,
				Name = name
			};
		}
	}
}
