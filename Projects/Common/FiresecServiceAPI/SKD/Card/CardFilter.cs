using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class CardFilter : IsDeletedFilter
	{
		[DataMember]
		public List<Guid> EmployeeUIDs { get; set; }

		[DataMember]
		public DeletedType WithBlocked { get; set; }

		public CardFilter():base()
		{
			EmployeeUIDs = new List<Guid>();
			WithBlocked = DeletedType.Not;
		}
	}
}