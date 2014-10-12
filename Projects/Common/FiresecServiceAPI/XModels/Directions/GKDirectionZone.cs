using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Зона направления
	/// </summary>
	[DataContract]
	public class GKDirectionZone
	{
		public GKDirectionZone()
		{
			StateBit = GKStateBit.Fire1;
		}

		[XmlIgnore]
		public GKZone Zone { get; set; }

		/// <summary>
		/// Идентификатор зоны
		/// </summary>
		[DataMember]
		public Guid ZoneUID { get; set; }

		/// <summary>
		/// Бит состояния
		/// </summary>
		[DataMember]
		public GKStateBit StateBit { get; set; }
	}
}