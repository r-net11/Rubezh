using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.GK
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
			Device = new GKDevice();
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
		[Description("Постановка на охрану")]
		SetGuard,

		[Description("Снятие с охраны")]
		ResetGuard,

		[Description("Изменение")]
		ChangeGuard,

		[Description("Тревожный датчик")]
		SetAlarm,
	}
}