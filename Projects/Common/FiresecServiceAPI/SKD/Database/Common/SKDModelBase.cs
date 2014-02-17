using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public abstract class SKDModelBase
	{
		public SKDModelBase()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public bool? IsDeleted { get; set; }

		[DataMember]
		public DateTime? RemovalDate { get; set; } 
	}
}
