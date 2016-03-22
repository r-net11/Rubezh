using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class CloseDialogArguments : UIArguments
	{
		public CloseDialogArguments()
		{
			WindowIDArgument = new Argument();
			LayoutFilter.Add(System.Guid.Empty);
		}

		[DataMember]
		public Argument WindowIDArgument { get; set; }
	}
}
