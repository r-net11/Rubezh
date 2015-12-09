﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartPlansProperties : ILayoutProperties
	{
		[DataMember]
		public List<Guid> Plans { get; set; }
		[DataMember]
		public LayoutPartPlansType Type { get; set; }
	}
}