using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class TimeTrackDocumentType
	{
		public TimeTrackDocumentType()
		{
			UID = Guid.NewGuid();
		}

		public TimeTrackDocumentType(string name, string shortName, int code, DocumentType type)
		{
			Name = name;
			ShortName = shortName;
			Code = code;
			DocumentType = type;
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string ShortName { get; set; }

		[DataMember]
		public int Code { get; set; }

		[DataMember]
		public bool IsSystem { get; set; }

		[DataMember]
		public DocumentType DocumentType { get; set; }

		[DataMember]
		public Guid OrganisationUID { get; set; }
	}
}