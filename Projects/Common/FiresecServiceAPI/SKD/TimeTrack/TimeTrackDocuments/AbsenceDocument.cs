using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class AbsenceDocument : ITimeTrackDocument
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

		public AbsenceDocument()
		{
			TimeTrackDocumentType = new TimeTrackDocumentType
			{
				DocumentType = DocumentType.Absence
			};
		}

		public AbsenceDocument(string name, string shortName, int code)
		{
			TimeTrackDocumentType = new TimeTrackDocumentType
			{
				Name = name,
				ShortName = shortName,
				Code = code,
				DocumentType = DocumentType.Absence
			};
		}
	}
}
