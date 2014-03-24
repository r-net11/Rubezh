using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class AdditionalColumnFilter : IsDeletedFilter
	{
		[DataMember]
		public List<Guid> EmployeeUIDs { get; set; }

		[DataMember]
		public List<Guid> ColumnTypeUIDs { get; set; }

		public AdditionalColumnFilter()
			: base()
		{
			EmployeeUIDs = new List<Guid>();
			ColumnTypeUIDs = new List<Guid>();
		}
	}
}