using System;
using System.Runtime.Serialization;
using FiresecAPI.Automation;

namespace FiresecAPI.AutomationCallback
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
