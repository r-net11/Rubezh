using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartSKDJournalProperties : ILayoutProperties
	{
		[DataMember]
		public Guid FilterUID { get; set; }
	}
}