using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ControlSKDZoneArguments
	{
		public ControlSKDZoneArguments()
		{
			SKDZoneParameter = new Variable();
		}

		[DataMember]
		public Variable SKDZoneParameter { get; set; }

		[DataMember]
		public SKDZoneCommandType SKDZoneCommandType { get; set; }
	}

	public enum SKDZoneCommandType
	{
		[Description("Открыть все двери")]
		Open,

		[Description("Закрыть все двери")]
		Close,
		
		[Description("Установить режим ОТКРЫТО")]
		OpenForever,

		[Description("Установить режим ЗАКРЫТО")]
		CloseForever,

		[Description("Обнаружение сотрудников")]
		DetectEmployees
	}
}