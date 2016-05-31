using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public abstract class FilterBase
	{
		protected FilterBase()
		{
			UIDs = new List<Guid>();
			ExceptUIDs = new List<Guid>();
		}

		[DataMember]
		public List<Guid> UIDs { get; set; }

		[DataMember]
		public List<Guid> ExceptUIDs { get; set; }
	}
}