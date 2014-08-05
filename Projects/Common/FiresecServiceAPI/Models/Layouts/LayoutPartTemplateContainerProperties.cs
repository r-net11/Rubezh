using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartTemplateContainerProperties : ILayoutProperties
	{
		[DataMember]
		public Guid SourceUID { get; set; }
	}
}