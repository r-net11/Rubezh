using RubezhAPI.Plans.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Зона СКД
	/// </summary>
	[DataContract]
	public class GKSKDZone : GKBase, IPlanPresentable
	{
		public GKSKDZone()
		{
			PlanElementUIDs = new List<Guid>();
			Devices = new List<GKDevice>();
		}

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }

		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.SKDZone; } }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		[XmlIgnore]
		public override string ImageSource
		{
			get { return "/Controls;component/Images/SKDZone.png"; }
		}
	}
}