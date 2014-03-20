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
			AdditionalColumns = new List<JournalColumnType>();
			Count = 1;
			PageSize = 100;
		}

		[DataMember]
		public ArchiveDefaultStateType ArchiveDefaultStateType { get; set; }

		[DataMember]
		public int? Count { get; set; }

		[DataMember]
		public DateTime? StartDate { get; set; }

		[DataMember]
		public DateTime? EndDate { get; set; }

		[DataMember]
		public List<JournalColumnType> AdditionalColumns { get; set; }

		[DataMember]
		public int PageSize { get; set; }
	}
}