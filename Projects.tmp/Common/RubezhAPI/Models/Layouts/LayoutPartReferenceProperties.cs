using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartReferenceProperties : ILayoutProperties
	{
		[DataMember]
		public Guid ReferenceUID { get; set; }
		public Guid? ReferenceSVGUID { get; set; }
	}
}