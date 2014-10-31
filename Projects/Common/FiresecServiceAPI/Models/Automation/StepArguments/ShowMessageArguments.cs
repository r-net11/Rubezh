using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ShowMessageArguments : UIArguments
	{
		public ShowMessageArguments()
		{
			MessageArgument = new Argument();
		}

		[DataMember]
		public Argument MessageArgument { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[DataMember]
		public bool IsModalWindow { get; set; }
	}
}