using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Устройство направления
	/// </summary>
	[DataContract]
	public class GKDirectionDevice
	{
		[XmlIgnore]
		public GKDevice Device { get; set; }

		/// <summary>
		/// Идентификатор устройства
		/// </summary>
		[DataMember]
		public Guid DeviceUID { get; set; }

		/// <summary>
		/// Бит состояния
		/// </summary>
		[DataMember]
		public GKStateBit StateBit { get; set; }
	}
}