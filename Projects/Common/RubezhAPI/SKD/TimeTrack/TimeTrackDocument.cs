﻿using System;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class TimeTrackDocument
	{
		public TimeTrackDocument()
		{
			UID = Guid.NewGuid();
			DocumentCode = 0;
			StartDateTime = DateTime.Now.Date;
			EndDateTime = DateTime.Now.Date;
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

		[DataMember]
		public TimeTrackDocumentType TimeTrackDocumentType { get; set; }

		[DataMember]
		public string FileName { get; set; }
	}
}