using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public abstract class SKDIsDeletedModel : SKDModelBase
	{
		[DataMember]
		public bool IsDeleted { get; set; }

		[DataMember]
		public DateTime RemovalDate { get; set; }
	}

	[DataContract]
	public abstract class SKDModelBase
	{
		public SKDModelBase()
		{
			UID = Guid.NewGuid();
		}

		[DataMember]
		public Guid UID { get; set; }
	}
}