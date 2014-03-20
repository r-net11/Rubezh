using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDCard : SKDIsDeletedModel
	{
		public SKDCard()
		{
			CardZones = new List<CardZone>();
		}

		[DataMember]
		public int Series { get; set; }

		[DataMember]
		public int Number { get; set; }

		[DataMember]
		public Guid? HolderUID { get; set; }

		[DataMember]
		public DateTime ValidFrom { get; set; }

		[DataMember]
		public DateTime ValidTo { get; set; }

		[DataMember]
		public List<CardZone> CardZones { get; set; }

		[DataMember]
		public Guid? CardTemplateUID { get; set; }

		[DataMember]
		public Guid? AccessTemplateUID { get; set; }

		[DataMember]
		public bool IsInStopList { get; set; }

		[DataMember]
		public string StopReason { get; set; }

		public string PresentationName
		{
			get { return Series.ToString() + "/" + Number; }
		}
	}
}