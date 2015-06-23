using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.GK;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.GK
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

		public override void Update(GKDevice device)
		{

		}

		public override void Update(GKDirection direction)
		{

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