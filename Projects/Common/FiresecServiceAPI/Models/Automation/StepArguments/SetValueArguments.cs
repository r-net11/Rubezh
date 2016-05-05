using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class SetValueArguments
	{
		public SetValueArguments()
		{
			SourceArgument = new Argument();
			TargetArgument = new Argument();
		}

		[DataMember]
		public Argument TargetArgument { get; set; }

		[DataMember]
		public Argument SourceArgument { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }
	}
}