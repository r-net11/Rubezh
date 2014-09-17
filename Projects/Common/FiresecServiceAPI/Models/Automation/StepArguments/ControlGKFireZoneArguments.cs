using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ControlGKFireZoneArguments
	{
		public ControlGKFireZoneArguments()
		{
			GKFireZoneParameter = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter GKFireZoneParameter { get; set; }

		[DataMember]
		public ZoneCommandType ZoneCommandType { get; set; }
	}

	public enum ZoneCommandType
	{
		[Description("Отключить")]
		Ignore,

		[Description("Снять отключение")]
		ResetIgnore,

		[Description("Сбросить")]
		Reset
	}
}