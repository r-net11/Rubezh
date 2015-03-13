using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Охранная зона устройства
	/// </summary>
	[DataContract]
	public class GKDeviceGuardZone
	{
		public GKDeviceGuardZone()
		{
			GuardZone = new GKGuardZone();
			CodeReaderSettings = new GKCodeReaderSettings();
		}

		[XmlIgnore]
		public GKGuardZone GuardZone { get; set; }

		/// <summary>
		/// Идентификатор охранный зоны
		/// </summary>
		[DataMember]
		public Guid GuardZoneUID { get; set; }

		/// <summary>
		/// Тип действия
		/// </summary>
		[DataMember]
		public GKGuardZoneDeviceActionType? ActionType { get; set; }

		/// <summary>
		/// Настройки кодонаборника в охранной зоне
		/// </summary>
		[DataMember]
		public GKCodeReaderSettings CodeReaderSettings { get; set; }
	}
}