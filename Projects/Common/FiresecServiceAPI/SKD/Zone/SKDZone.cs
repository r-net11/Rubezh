﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Common;
using FiresecAPI.GK;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDZone : ModelBase, IStateProvider, IPlanPresentable
	{
		public SKDZone()
		{
			PlanElementUIDs = new List<Guid>();
			Devices = new List<SKDDevice>();
		}

		[XmlIgnore]
		public SKDZoneState State { get; set; }
		[XmlIgnore]
		public List<SKDDevice> Devices { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		#region IStateProvider Members
		IDeviceState IStateProvider.StateClass
		{
			get { return State; }
		}
		#endregion
	}
}