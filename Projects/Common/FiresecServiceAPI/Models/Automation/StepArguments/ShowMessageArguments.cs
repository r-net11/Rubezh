using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract, Serializable]
	public class ShowMessageArguments
	{
		public ShowMessageArguments()
		{
			MessageArgument = new Argument();
			ProcedureLayoutCollection = new ProcedureLayoutCollection();
		}

		[DataMember]
		public Argument MessageArgument { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[DataMember]
		public bool IsModalWindow { get; set; }

		[DataMember]
		public ProcedureLayoutCollection ProcedureLayoutCollection { get; set; }
	}
}