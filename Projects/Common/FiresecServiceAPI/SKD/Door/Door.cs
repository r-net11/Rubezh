using Common;
using StrazhAPI.Plans.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using StrazhAPI.GK;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDDoor : ModelBase, IStateProvider, IDeviceState, IPlanPresentable
	{
		public SKDDoor()
		{
			PlanElementUIDs = new List<Guid>();
		}

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

		IDeviceState IStateProvider.StateClass
		{
			get { return State; }
		}

		string IDeviceState.Name
		{
			get { return State.StateClass.ToDescription(); }
		}

		#endregion IStateProvider Members

		#region IDeviceState<XStateClass> Members

		XStateClass IDeviceState.StateClass
		{
			get { return State.StateClass; }
		}

		event Action IDeviceState.StateChanged
		{
			add { }
			remove { }
		}

		#endregion IDeviceState<XStateClass> Members
	}
}