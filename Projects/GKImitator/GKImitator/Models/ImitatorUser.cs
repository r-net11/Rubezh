using FiresecAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GKImitator.Processor
{
	[DataContract]
	public class ImitatorUser
	{
		public ImitatorUser()
		{
			ImitatorUserDevices = new List<ImitatorUserDevice>();
		}

		[DataMember]
		public int GKNo { get; set; }

		[DataMember]
		public int Number { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public DateTime EndDateTime { get; set; }

		[DataMember]
		public int TotalSeconds { get; set; }

		[DataMember]
		public GKCardType CardType { get; set; }

		[DataMember]
		public int Level { get; set; }

		[DataMember]
		public int ScheduleNo { get; set; }

		[DataMember]
		public bool IsBlocked { get; set; }

		[DataMember]
		public List<ImitatorUserDevice> ImitatorUserDevices { get; set; }
	}

	[DataContract]
	public class ImitatorUserDevice
	{
		[DataMember]
		public int DescriptorNo { get; set; }

		[DataMember]
		public int ScheduleNo { get; set; }
	}
}