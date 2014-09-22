using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class SetValueArguments
	{
		public SetValueArguments()
		{
			SourceParameter = new Variable();
			TargetParameter = new Variable();
		}

		[DataMember]
		public Variable TargetParameter { get; set; }

		[DataMember]
		public Variable SourceParameter { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

	}
}
