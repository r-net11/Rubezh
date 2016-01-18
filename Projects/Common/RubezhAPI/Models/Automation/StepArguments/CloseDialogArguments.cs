using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class CloseDialogArguments : UIArguments
	{
		public CloseDialogArguments()
		{
			WindowIDArgument = new Argument();
		}

		[DataMember]
		public Argument WindowIDArgument { get; set; }
	}
}
