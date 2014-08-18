using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ExitCode
	{
		public ExitCode()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public VariableType VariableType { get; set; }

		[DataMember]
		public ExitCodeType ExitCodeType { get; set; }

		[DataMember]
		public Guid GlobalVariableUid { get; set; }

		[DataMember]
		public Guid VariableUid { get; set; }

	}

	public enum ExitCodeType
	{
		[Description("Нормальное завершение")]
		Normal,

		[Description("Досрочное завершение")]
		Interrupt
	}
}