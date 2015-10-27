using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class TimeTrackDocumentType
	{
		public TimeTrackDocumentType()
		{
			UID = Guid.NewGuid();
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
		public DocumentType DocumentType { get; set; }

		[DataMember]
		public Guid OrganisationUID { get; set; }
	}
}