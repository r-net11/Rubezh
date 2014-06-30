using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using Common;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDZone : IStateProvider, IIdentity, IPlanPresentable
	{
		public SKDZone()
		{
			UID = Guid.NewGuid();
			PlanElementUIDs = new List<Guid>();
			Devices = new List<SKDDevice>();
		}

		public SKDZone Parent { get; set; }
		public SKDZoneState State { get; set; }
		public List<SKDDevice> Devices { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }
		
		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;

		#region IStateProvider Members

		IDeviceState<XStateClass> IStateProvider.StateClass
		{
			get { return State; }
		}

		#endregion
	}
}