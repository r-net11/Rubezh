using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Направление ГК
	/// </summary>
	[DataContract]
	public class GKDirection : GKBase, IPlanPresentable
	{
		public GKDirection()
		{
			DirectionZones = new List<GKDirectionZone>();
			DirectionDevices = new List<GKDirectionDevice>();
			DelayRegime = DelayRegime.Off;

			InputDevices = new List<GKDevice>();
			InputZones = new List<GKZone>();
			OutputDevices = new List<GKDevice>();
			PlanElementUIDs = new List<Guid>();
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Direction; } }

		[XmlIgnore]
		public List<GKDevice> InputDevices { get; set; }
		[XmlIgnore]
		public List<GKZone> InputZones { get; set; }
		[XmlIgnore]
		public List<GKDevice> OutputDevices { get; set; }

		/// <summary>
		/// Задержка на включение
		/// </summary>
		[DataMember]
		public ushort Delay { get; set; }

		/// <summary>
		/// Время удержания
		/// </summary>
		[DataMember]
		public ushort Hold { get; set; }

		[DataMember]
		public DelayRegime DelayRegime { get; set; }

		/// <summary>
		/// Зоны направления
		/// </summary>
		[DataMember]
		public List<GKDirectionZone> DirectionZones { get; set; }

		/// <summary>
		/// Устройства направления
		/// </summary>
		[DataMember]
		public List<GKDirectionDevice> DirectionDevices { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }
	}
}