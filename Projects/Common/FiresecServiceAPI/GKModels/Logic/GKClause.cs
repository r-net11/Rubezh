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
			DirectionUIDs = new List<Guid>();
			MPTUIDs = new List<Guid>();
			DelayUIDs = new List<Guid>();
			DoorUIDs = new List<Guid>();

			Devices = new List<GKDevice>();
			Directions = new List<GKDirection>();
			Delays = new List<GKDelay>();
			Doors = new List<GKDoor>();
			MPTs = new List<GKMPT>();
		}

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }
		[XmlIgnore]
		public List<GKDirection> Directions { get; set; }
		[XmlIgnore]
		public List<GKDelay> Delays { get; set; }
		[XmlIgnore]
		public List<GKDoor> Doors { get; set; }
		[XmlIgnore]
		public List<GKMPT> MPTs { get; set; }

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
		/// Идентификаторы направлений
		/// </summary>
		[DataMember]
		public List<Guid> DirectionUIDs { get; set; }

		/// <summary>
		/// Идентификаторы задержек
		/// </summary>
		[DataMember]
		public List<Guid> DelayUIDs { get; set; }

		/// <summary>
		/// Идентификаторы точек доступа
		/// </summary>
		[DataMember]
		public List<Guid> DoorUIDs { get; set; }

		/// <summary>
		/// Идентификаторы МПТ
		/// </summary>
		[DataMember]
		public List<Guid> MPTUIDs { get; set; }

		/// <summary>
		/// Тип операции
		/// </summary>
		[DataMember]
		public ClauseOperationType ClauseOperationType { get; set; }

		public bool HasObjects()
		{
			return Devices.Count > 0 || Directions.Count > 0 || MPTs.Count > 0 || Delays.Count > 0 || Doors.Count > 0;
		}

		public static string ClauseToString(ClauseOperationType clauseOperationType, GKStateBit stateBit)
		{
			switch (clauseOperationType)
			{
				case ClauseOperationType.AllDoors:
				case ClauseOperationType.AnyDoor:
					switch (stateBit)
					{
						case GKStateBit.Fire1:
							return "Тревога";
					}
					break;
			}

			return stateBit.ToDescription();
		}
	}
}