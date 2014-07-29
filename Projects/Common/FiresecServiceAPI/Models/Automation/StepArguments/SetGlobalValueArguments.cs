using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class SetGlobalValueArguments
	{
		public SetGlobalValueArguments()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid GlobalVariableUid { get; set; }

		[DataMember]
		public int Value { get; set; }
	}
}
