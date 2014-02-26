using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class CardFilter : FilterBase
	{
		[DataMember]
		public List<Guid> EmployeeUids { get; set; }

		[DataMember]
		public DeletedType WithBlocked { get; set; }

		public CardFilter():base()
		{
			EmployeeUids = new List<Guid>();
			WithBlocked = DeletedType.Not;
		}
	}
}