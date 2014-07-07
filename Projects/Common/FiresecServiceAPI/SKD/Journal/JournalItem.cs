using System;
using System.Runtime.Serialization;
using FiresecAPI.Events;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class JournalItem : SKDModelBase
	{
		public JournalItem():base()
		{
			DeviceDateTime = DateTime.Now;
			SystemDateTime = DateTime.Now;
			State = XStateClass.Norm;
		}

		[DataMember]
		public DateTime SystemDateTime { get; set; }

		[DataMember]
		public DateTime DeviceDateTime { get; set; }
		
		[DataMember]
		public SubsystemType SubsystemType { get; set; }
		
		[DataMember]
		public GlobalEventNameEnum Name { get; set; }

		[DataMember]
		public EventDescription Description { get; set; }

		[DataMember]
		public string NameText { get; set; }

		[DataMember]
		public string DescriptionText { get; set; }

		[DataMember]
		public XStateClass State { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		[DataMember]
		public string ObjectName { get; set; }

		[DataMember]
		public Guid ObjectUID { get; set; }

		[DataMember]
		public string UserName { get; set; }

		[DataMember]
		public int CardNo { get; set; }
	}
}