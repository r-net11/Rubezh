﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.AutomationCallback
{
	[DataContract]
	public class ProcedureProperties
	{
		[DataMember]
		public List<VisualPropertyCallbackData> VisualProperties { get; set; }
		[DataMember]
		public List<PlanCallbackData> PlanProperties { get; set; }
	}
}
