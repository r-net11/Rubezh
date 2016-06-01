using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class AdditionalColumnFilter : FilterBase
	{
		[DataMember]
		public List<Guid> EmployeeUIDs { get; set; }

		[DataMember]
		public List<Guid> ColumnTypeUIDs { get; set; }

		public AdditionalColumnFilter()
		{
			EmployeeUIDs = new List<Guid>();
			ColumnTypeUIDs = new List<Guid>();
		}
	}
}