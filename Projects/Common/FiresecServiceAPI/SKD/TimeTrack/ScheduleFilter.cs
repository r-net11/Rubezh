﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class ScheduleFilter : EmployeeFilterBase
	{
		public ScheduleFilter()
			: base()
		{
			ScheduleSchemeUIDs = new List<Guid>();
		}

		[DataMember]
		public List<Guid> ScheduleSchemeUIDs { get; set; }
	}
}