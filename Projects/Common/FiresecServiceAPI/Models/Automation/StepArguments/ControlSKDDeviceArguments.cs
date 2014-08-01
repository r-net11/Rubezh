using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ControlSKDDeviceArguments
	{
		public ControlSKDDeviceArguments()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid DeviceUid { get; set; }

		[DataMember]
		public SKDDeviceCommandType Command { get; set; }
	}
	
	public enum SKDDeviceCommandType
	{
		[Description("Открыть")]
		Open,

		[Description("Закрыть")]
		Close,

		[Description("Установить режим ОТКРЫТО")]
		OpenForever,

		[Description("Установить режим ЗАКРЫТО")]
		CloseForever
	}
}

