using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Зона ГК
	/// </summary>
	[DataContract]
	public class GKZone : GKBase, IPlanPresentable
	{
		public GKZone()
		{
			Fire1Count = 2;
			Fire2Count = 3;
			PlanElementUIDs = new List<Guid>();
			Devices = new List<GKDevice>();
			Directions = new List<GKDirection>();
			DevicesInLogic = new List<GKDevice>();
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Zone; } }

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }
		[XmlIgnore]
		public List<GKDirection> Directions { get; set; }
		[XmlIgnore]
		public List<GKDevice> DevicesInLogic { get; set; }

		/// <summary>
		/// Количество устройств в сработке-1 для перевода зоны в состояние Пожар-1
		/// </summary>
		[DataMember]
		public int Fire1Count { get; set; }

		/// <summary>
		/// Количество устройств в сработке-2 для перевода зоны в состояние Пожар-2
		/// </summary>
		[DataMember]
		public int Fire2Count { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		[XmlIgnore]
		public List<Guid> PlanElementUIDs { get; set; }
	}
}