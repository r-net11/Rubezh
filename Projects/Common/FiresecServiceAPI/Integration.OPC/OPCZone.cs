using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Common;
using StrazhAPI.Enums;
using System.Runtime.Serialization;
using StrazhAPI.GK;
using StrazhAPI.Plans.Interfaces;
using StrazhAPI.SKD;

namespace StrazhAPI.Integration.OPC
{
	[DataContract]
	public class OPCZone : ModelBase, IStateProvider, IPlanPresentable
	{
		public OPCZone()
		{
			PlanElementUIDs = new List<Guid>();
		}

		[DataMember]
		public OPCZoneType? Type { get; set; }

		[DataMember]
		public GuardZoneType? GuardZoneType { get; set; }

		[DataMember]
		public bool? IsSkippedTypeEnabled { get; set; }

		[DataMember]
		public int? Delay { get; set; }

		[DataMember]
		public int? AutoSet { get; set; }

		[XmlIgnore]
		public SKDZoneState State { get; set; }

		#region IStateProvider Members
		IDeviceState IStateProvider.StateClass
		{
			get { return State; }
		}
		#endregion IStateProvider Members

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }
	}
}
