using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class GlobalVariable
	{
		public GlobalVariable()
		{
			Name = "Новая глобальная переменная";
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Value { get; set; }

		[DataMember]
		public Guid Uid { get; set; }
	}
}
