﻿using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartSKDVerificationProperties : ILayoutProperties
	{
		public LayoutPartSKDVerificationProperties()
		{
			JournalFilters = new List<SKDJournalFilter>();
		}

		[DataMember]
		public Guid ReaderDeviceUID { get; set; }

		[DataMember]
		public bool ShowEmployeeCardID { get; set; }

		[DataMember]
		public bool ShowEmployeeName { get; set; }

		[DataMember]
		public bool ShowEmployeePassport { get; set; }

		[DataMember]
		public bool ShowEmployeeTime { get; set; }

		[DataMember]
		public bool ShowEmployeeNo { get; set; }

		[DataMember]
		public bool ShowEmployeePosition { get; set; }

		[DataMember]
		public bool ShowEmployeeShedule { get; set; }

		[DataMember]
		public bool ShowEmployeeDepartment { get; set; }

		[DataMember]
		public bool ShowGuestCardID { get; set; }

		[DataMember]
		public bool ShowGuestName { get; set; }

		[DataMember]
		public bool ShowGuestWhere { get; set; }

		[DataMember]
		public bool ShowGuestConvoy { get; set; }

		[DataMember]
		public List<SKDJournalFilter> JournalFilters { get; set; }
	}
}