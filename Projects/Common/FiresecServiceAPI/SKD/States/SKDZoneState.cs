﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDZoneState : IDeviceState<XStateClass>
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

		XStateClass IDeviceState<XStateClass>.StateType
		{
			get { return StateClass; }
		}
		string IDeviceState<XStateClass>.StateTypeName
		{
			get { return StateClass.ToDescription(); }
		}

		#endregion
	}
}