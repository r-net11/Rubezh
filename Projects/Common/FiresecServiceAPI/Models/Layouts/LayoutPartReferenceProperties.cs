using System;
using System.Runtime.Serialization;

namespace StrazhAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartReferenceProperties : ILayoutProperties
	{
		[DataMember]
		public Guid ReferenceUID { get; set; }
	}
}