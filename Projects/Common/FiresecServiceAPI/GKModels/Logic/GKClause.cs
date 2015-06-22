using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Условия
	/// </summary>
	[DataContract]
	public class GKClause
	{
		public GKClause()
		{
			StateType = GKStateBit.Fire1;
			DeviceUIDs = new List<Guid>();
			DoorUIDs = new List<Guid>();

			Devices = new List<GKDevice>();
		}

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }

		/// <summary>
		/// Тип условия
		/// </summary>
		[DataMember]
		public ClauseConditionType ClauseConditionType { get; set; }

		/// <summary>
		/// Бит состояния
		/// </summary>
		[DataMember]
		public GKStateBit StateType { get; set; }

		/// <summary>
		/// Идентификаторы устройств
		/// </summary>
		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }

		/// <summary>
		/// Идентификаторы точек доступа
		/// </summary>
		[DataMember]
		public List<Guid> DoorUIDs { get; set; }

		/// <summary>
		/// Тип операции
		/// </summary>
		[DataMember]
		public ClauseOperationType ClauseOperationType { get; set; }

		public bool HasObjects()
		{
			return Devices.Count > 0;
		}

		public static string ClauseToString(ClauseOperationType clauseOperationType, GKStateBit stateBit)
		{
			return stateBit.ToDescription();
		}
	}
}