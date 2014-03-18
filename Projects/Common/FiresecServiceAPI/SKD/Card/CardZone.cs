using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class CardZone: SKDIsDeletedModel
	{
		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public bool IsAntiPassback { get; set; }

		[DataMember]
		public bool IsComission { get; set; }

		[DataMember]
		public IntervalType IntervalType { get; set; }

		[DataMember]
		public Guid? IntervalUID { get; set; }

		[DataMember]
		public Guid? ParentUID { get; set; }

		[DataMember]
		public ParentType ParentType { get; set; }
	}

	public enum ParentType
	{
		Card,
		GUDExceptons,
		GUDAdditions,
		GUD
	}
}