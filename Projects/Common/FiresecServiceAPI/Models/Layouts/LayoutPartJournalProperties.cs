using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartJournalProperties : ILayoutProperties
	{
		[DataMember]
		public Guid FilterUID { get; set; }
	}
}