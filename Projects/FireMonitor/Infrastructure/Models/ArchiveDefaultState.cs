using System;
using System.Runtime.Serialization;

namespace Infrastructure.Models
{
	[DataContract]
	public class ArchiveDefaultState
	{
		public ArchiveDefaultState()
		{
			ArchiveDefaultStateType = ArchiveDefaultStateType.LastDays;
			Count = 1;
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
		public bool ShowIP { get; set; }

		[DataMember]
		public bool ShowSubsystem { get; set; }
	}
}