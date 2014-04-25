using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public abstract class FilterBase
	{
		public FilterBase()
		{
			UIDs = new List<Guid>();
		}

		[DataMember]
		public List<Guid> UIDs { get; set; }
	}
}