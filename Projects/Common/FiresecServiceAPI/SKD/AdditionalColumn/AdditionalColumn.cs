using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class AdditionalColumn : SKDIsDeletedModel
	{
		[DataMember]
		public Guid? EmployeeUID { get; set; }

		[DataMember]
		public Guid? PhotoUID { get; set; }

		[DataMember]
		public Guid? AdditionalColumnTypeUID { get; set; }

		[DataMember]
		public string TextData { get; set; }
	}
}