using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class GetStringArguments
	{
		public GetStringArguments()
		{
			ResultVariableUid = new Guid();
			VariableUid = new Guid();
		}

		[DataMember]
		public Guid ResultVariableUid { get; set; }

		[DataMember]
		public Guid VariableUid { get; set; }

		[DataMember]
		public StringOperation StringOperation { get; set; }

		[DataMember]
		public Property Property { get; set; }
	}

	public enum StringOperation
	{
		[Description("Присвоить")]
		Is,

		[Description("Добавить")]
		Add
	}
}