using System.Runtime.Serialization;
using System;
using System.Collections.Generic;

namespace FiresecAPI
{
	[DataContract]
	public abstract class FilterBase
	{
		[DataMember]
		public List<Guid> Uids { get; set; }

		public FilterBase()
		{
			Uids = new List<Guid>();
		}
	}

}