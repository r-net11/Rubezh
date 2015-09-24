using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Infrastructure.Models
{
	[DataContract]
	public class ArchiveDefaultState
	{
		public ArchiveDefaultState()
		{
			ArchiveDefaultStateType = ArchiveDefaultStateType.LastDays;
			XAdditionalColumns = new List<XJournalColumnType>();
			AdditionalJournalColumnTypes = new List<JournalColumnType>();
			Count = 1;
			PageSize = 100;
		}

		[DataMember]
		public ArchiveDefaultStateType ArchiveDefaultStateType { get; set; }

		[DataMember]
		public int Count { get; set; }

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public bool UseDeviceDateTime { get; set; }

		[DataMember]
		public bool IsSortAsc { get; set; }

		[DataMember]
		public List<XJournalColumnType> XAdditionalColumns { get; set; }

		[DataMember]
		public List<JournalColumnType> AdditionalJournalColumnTypes { get; set; }

		[DataMember]
		public int PageSize { get; set; }
	}
}