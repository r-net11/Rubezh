using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class IncrementGlobalValueArguments
	{
		public IncrementGlobalValueArguments()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid GlobalVariableUid { get; set; }

		[DataMember]
		public IncrementType IncrementType { get; set; }
	}

	public enum IncrementType
	{
		[Description("Инкремент")]
		Inc,

		[Description("Декремент")]
		Dec,
	}
}
