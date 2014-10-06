using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.GK;
using Infrustructure.Plans.Interfaces;
using System.Xml.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDoor : ModelBase, IStateProvider, IDeviceState<XStateClass>, IPlanPresentable
	{
		[XmlIgnore]
		public SKDDoorState State { get; set; }

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