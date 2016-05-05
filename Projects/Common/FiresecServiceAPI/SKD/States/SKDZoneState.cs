using Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using StrazhAPI.GK;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDZoneState : IDeviceState
	{
		public SKDZoneState()
		{
			StateClasses = new List<XStateClass>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<XStateClass> StateClasses { get; set; }

		[DataMember]
		public XStateClass StateClass { get; set; }

		public SKDZone Zone { get; private set; }

		public SKDZoneState(SKDZone zone)
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

		#endregion IDeviceState<XStateClass> Members
	}
}