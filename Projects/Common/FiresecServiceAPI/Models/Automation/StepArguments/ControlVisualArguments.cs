using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ControlVisualArguments
	{
		public ControlVisualArguments()
		{
			LayoutUid = new Guid();
		}

		[DataMember]
		public Guid LayoutUid { get; set; }

		[DataMember]
		public string ElementName { get; set; }

		[DataMember]
		public string ElementProperty { get; set; }

		[DataMember]
		public string PropertyValue { get; set; }
	}
}