using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class SetJournalItemGuidArguments
	{
		public SetJournalItemGuidArguments()
		{
			ValueArgument = new Argument();
		}

		[DataMember]
		public Argument ValueArgument { get; set; }
	}
}