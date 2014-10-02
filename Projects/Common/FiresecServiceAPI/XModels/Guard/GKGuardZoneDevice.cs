using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKGuardZoneDevice
	{
		public GKGuardZoneDevice()
		{
			CodeReaderSettings = new GKCodeReaderSettings();
		}

		[XmlIgnore]
		public GKDevice Device { get; set; }

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public GKGuardZoneDeviceActionType ActionType { get; set; }

		[DataMember]
		public GKCodeReaderSettings CodeReaderSettings { get; set; }
	}

	public enum GKGuardZoneDeviceActionType
	{
		[Description("Поставка на охрану")]
		SetGuard,

		[Description("Снятие с охраны")]
		ResetGuard,

		[Description("Тревожный датчик")]
		SetAlarm,
	}
}