using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class SetValueArguments
	{
		public SetValueArguments()
		{
			Uid = Guid.NewGuid();
			SourceParameter = new ArithmeticParameter();
			TargetParameter = new ArithmeticParameter();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public ArithmeticParameter TargetParameter { get; set; }

		[DataMember]
		public ArithmeticParameter SourceParameter { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

	}
}
