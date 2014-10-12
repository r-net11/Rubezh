using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Устройство охранной зоны
	/// </summary>
	[DataContract]
	public class GKGuardZoneDevice
	{
		public GKGuardZoneDevice()
		{
			CodeReaderSettings = new GKCodeReaderSettings();
		}

		[XmlIgnore]
		public GKDevice Device { get; set; }

		/// <summary>
		/// Идентификатор устройства
		/// </summary>
		[DataMember]
		public Guid DeviceUID { get; set; }

		/// <summary>
		/// Тип действия
		/// </summary>
		[DataMember]
		public GKGuardZoneDeviceActionType ActionType { get; set; }

		/// <summary>
		/// Настройки кодонаборника в охранной зоне
		/// </summary>
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