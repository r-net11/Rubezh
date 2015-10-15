﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using RubezhClient;
using Infrustructure.Plans.Interfaces;
using System.Linq;

namespace RubezhAPI.GK
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
		}

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Zone; } }

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }

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

		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/Zone.png"; }
		}
	}
}