using System;
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
		[DataMember]
		public bool ShowZoomSliders { get; set; }
		[DataMember]
		public double DeviceZoom { get; set; }
		[DataMember]
		public bool AllowChangePlanZoom { get; set; }
	}
}