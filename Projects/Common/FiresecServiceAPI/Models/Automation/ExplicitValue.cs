using System;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ExplicitValue
	{
		public ExplicitValue()
		{
			DateTimeValue = DateTime.Now;
			IntValue = 0;
			StringValue = "";
			UidValue = Guid.Empty;
		}

		[DataMember]
		public int IntValue { get; set; }

		[DataMember]
		public bool BoolValue { get; set; }

		[DataMember]
		public DateTime DateTimeValue { get; set; }

		[DataMember]
		public Guid UidValue { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public XStateClass StateTypeValue { get; set; }

		[DataMember]
		public GKDriverType DriverTypeValue { get; set; }
	}
}