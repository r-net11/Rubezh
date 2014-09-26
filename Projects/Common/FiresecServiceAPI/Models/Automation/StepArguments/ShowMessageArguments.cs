using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ShowMessageArguments
	{
		public ShowMessageArguments()
		{
			MessageParameter = new Argument();
			ProcedureLayoutCollection = new ProcedureLayoutCollection();
		}

		[DataMember]
		public Argument MessageParameter { get; set; }

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