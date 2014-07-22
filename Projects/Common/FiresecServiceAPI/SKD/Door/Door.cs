using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.GK;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDoor : IStateProvider, IDeviceState<XStateClass>, IIdentity, IPlanPresentable
	{
		public SKDDoor()
		{
			UID = Guid.NewGuid();
		}

		public SKDDoorState State { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public DoorType DoorType { get; set; }

		[DataMember]
		public Guid InDeviceUID { get; set; }

		[DataMember]
		public Guid OutDeviceUID { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		public SKDDevice InDevice { get; set; }
		public SKDDevice OutDevice { get; set; }

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

		#region IDeviceState<XStateClass> Members
		XStateClass IDeviceState<XStateClass>.StateType
		{
			get { return State.StateClass; }
		}
		event Action IDeviceState<XStateClass>.StateChanged
		{
			add { }
			remove { }
		}

		#endregion
	}
}