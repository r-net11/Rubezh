using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class SetValueArguments
	{
		public SetValueArguments()
		{
			SourceParameter = new Argument();
			TargetParameter = new Argument();
		}

		[DataMember]
		public Argument TargetParameter { get; set; }

		[DataMember]
		public Argument SourceParameter { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

	}
}