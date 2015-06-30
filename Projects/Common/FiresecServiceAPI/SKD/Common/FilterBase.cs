﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public abstract class FilterBase
	{
		public FilterBase()
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