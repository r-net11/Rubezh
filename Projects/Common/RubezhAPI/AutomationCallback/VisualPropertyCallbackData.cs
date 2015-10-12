using System;
using System.Runtime.Serialization;
using RubezhAPI.Automation;

namespace RubezhAPI.AutomationCallback
{
	[DataContract]
	public class VisualPropertyCallbackData : AutomationCallbackData
	{
		[DataMember]
		public Guid LayoutPart { get; set; }

		[DataMember]
		public LayoutPartPropertyName Property { get; set; }

		[DataMember]
		public object Value { get; set; }
	}
}
