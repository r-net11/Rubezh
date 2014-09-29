using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	public class XGuardZoneDevice
	{
		public XGuardZoneDevice()
		{
			CodeReaderSettings = new XCodeReaderSettings();
		}

		[XmlIgnore]
		public XDevice Device { get; set; }

		public Guid DeviceUID { get; set; }
		public XGuardZoneDeviceActionType ActionType { get; set; }
		public XCodeReaderSettings CodeReaderSettings { get; set; }
	}

	public enum XGuardZoneDeviceActionType
	{
		[Description("Поставка на охрану")]
		SetGuard,

		[Description("Снятие с охраны")]
		ResetGuard,

		[Description("Тревожный датчик")]
		SetAlarm,
	}
}