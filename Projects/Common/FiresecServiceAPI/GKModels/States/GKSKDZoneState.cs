using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.GK;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKSKDZoneState : IDeviceState
	{
		public GKSKDZoneState()
		{
			StateClasses = new List<XStateClass>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }

		public GKSKDZone Zone { get; private set; }

		public GKSKDZoneState(GKSKDZone zone)
			: this()
		{
			Zone = zone;
			UID = zone.UID;
		}

		public event Action StateChanged;
		public void OnStateChanged()
		{
			if (StateChanged != null)
				StateChanged();
		}

		#region IDeviceState<XStateClass> Members
		XStateClass IDeviceState.StateClass
		{
			get { return StateClass; }
		}
		string IDeviceState.Name
		{
			get { return StateClass.ToDescription(); }
		}
		#endregion
	}
}