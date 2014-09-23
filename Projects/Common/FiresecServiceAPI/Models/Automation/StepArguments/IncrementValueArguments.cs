using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class IncrementValueArguments
	{
		public IncrementValueArguments()
		{
			ResultParameter = new Variable();
		}

		[DataMember]
		public Variable ResultParameter { get; set; }

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
