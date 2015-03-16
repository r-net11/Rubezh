﻿using System;
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
			ZoneUIDs = new List<Guid>();
			GuardZoneUIDs = new List<Guid>();
			DirectionUIDs = new List<Guid>();
			MPTUIDs = new List<Guid>();
			DelayUIDs = new List<Guid>();
			DoorUIDs = new List<Guid>();

			Devices = new List<GKDevice>();
			Zones = new List<GKZone>();
			GuardZones = new List<GKGuardZone>();
			Directions = new List<GKDirection>();
			Delays = new List<GKDelay>();
			Doors = new List<GKDoor>();
			MPTs = new List<GKMPT>();
		}

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }
		[XmlIgnore]
		public List<GKZone> Zones { get; set; }
		[XmlIgnore]
		public List<GKGuardZone> GuardZones { get; set; }
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
		/// Идентификаторы зон
		/// </summary>
		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		/// <summary>
		/// Идентификаторы охрвнных зон
		/// </summary>
		[DataMember]
		public List<Guid> GuardZoneUIDs { get; set; }

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
			return Devices.Count > 0 || Zones.Count > 0 || GuardZones.Count > 0 || Directions.Count > 0 || MPTs.Count > 0 || Delays.Count > 0 || Doors.Count > 0;
		}

		public static string ClauseToString(ClauseOperationType clauseOperationType, GKStateBit stateBit)
		{
			switch (clauseOperationType)
			{
				case ClauseOperationType.AllZones:
				case ClauseOperationType.AnyZone:
					switch (stateBit)
					{
						case GKStateBit.Fire1:
							return "Пожар 1";

						case GKStateBit.Fire2:
							return "Пожар 2";

						case GKStateBit.Attention:
							return "Внимание";
					}
					break;

				case ClauseOperationType.AllGuardZones:
				case ClauseOperationType.AnyGuardZone:
					switch (stateBit)
					{
						case GKStateBit.On:
							return "На охране";

						case GKStateBit.Off:
							return "Не на охране";

						case GKStateBit.Fire1:
							return "Тревога";
					}
					break;
			}

			return stateBit.ToDescription();
		}
	}
}