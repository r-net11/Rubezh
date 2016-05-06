using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class GetObjectPropertyArguments
	{
		public GetObjectPropertyArguments()
		{
			ObjectArgument = new Argument();
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument ObjectArgument { get; set; }

		[DataMember]
		public Argument ResultArgument { get; set; }

		[DataMember]
		public Property Property { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }
	}
}