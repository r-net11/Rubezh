using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartPlansProperties : ILayoutProperties
	{
		[DataMember]
		public Guid SourceUID { get; set; }
		[DataMember]
		public LayoutPartPlansType Type { get; set; }
	}
}
