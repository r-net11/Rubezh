using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FiresecAPI
{
	[DataContract]
	public class SKDCard : SKDModelBase
	{
		public SKDCard()
		{
			CardZones = new List<CardZone>();
			AdditionalGUDZones = new List<CardZone>();
			ExceptedGUDZones = new List<CardZone>();
		}

		[DataMember]
		public int Series { get; set; }

		[DataMember]
		public int Number { get; set; }

		[DataMember]
		public Guid? HolderUid { get; set; }

		[DataMember]
		public DateTime ValidFrom { get; set; }

		[DataMember]
		public DateTime ValidTo { get; set; }

		[DataMember]
		public List<CardZone> CardZones { get; set; }

		[DataMember]
		public List<CardZone> AdditionalGUDZones { get; set; }

		[DataMember]
		public List<CardZone> ExceptedGUDZones { get; set; }

		[DataMember]
		public Guid? GUDUid { get; set; }

		[DataMember]
		public bool IsAntipass { get; set; }

		[DataMember]
		public bool IsInStopList { get; set; }

		[DataMember]
		public string StopReason { get; set; }
	}
}