using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class Door : IStateProvider, IDeviceState<XStateClass>, IIdentity
	{
		public Door()
		{
			UID = Guid.NewGuid();
		}

		public DoorState State { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid InDeviceUID { get; set; }

		[DataMember]
		public Guid OutDeviceUID { get; set; }

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