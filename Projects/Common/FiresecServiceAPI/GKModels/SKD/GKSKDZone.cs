using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.GK;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKSKDZone : ModelBase, IStateProvider, IPlanPresentable
	{
		public GKSKDZone()
		{
			PlanElementUIDs = new List<Guid>();
			Devices = new List<GKDevice>();
		}

		[XmlIgnore]
		public GKSKDZoneState State { get; set; }
		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		#region IStateProvider Members
		IDeviceState<XStateClass> IStateProvider.StateClass
		{
			get { return State; }
		}
		#endregion
	}
}