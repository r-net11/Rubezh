using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Position
	{
		public Position()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid;

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }
	}
}