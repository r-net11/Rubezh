using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Models.Layouts
{
	[DataContract]
	public class LayoutPartJournalProperties : ILayoutProperties
	{
		[DataMember]
		public Guid FilterUID { get; set; }
		public bool IsVisibleBottomPanel { get; set; }
	}
}
