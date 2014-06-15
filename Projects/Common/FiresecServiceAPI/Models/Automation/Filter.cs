using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class AutomationFilter
	{
		public AutomationFilter()
		{
			Uid = Guid.NewGuid();
			Name = "Новый фильтр";

			StartDate = DateTime.Now.AddDays(-1);
			EndDate = DateTime.Now;
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public DateTime StartDate { get; set; }

		[DataMember]
		public DateTime EndDate { get; set; }

		[DataMember]
		public bool UseDeviceDateTime { get; set; }
	}
}