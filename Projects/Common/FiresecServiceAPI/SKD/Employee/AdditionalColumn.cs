using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class AdditionalColumn : SKDModelBase
	{
		[DataMember]
		public Guid? EmployeeUID { get; set; }

		[DataMember]
		public Guid? AdditionalColumnTypeUID { get; set; }

		[DataMember]
		public string TextData { get; set; }

		[DataMember]
		public byte[] GraphicsData { get; set; }
	}
}