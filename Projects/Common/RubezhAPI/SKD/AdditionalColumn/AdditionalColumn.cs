using System;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class AdditionalColumn : SKDModelBase
	{
		[DataMember]
		public Guid? EmployeeUID { get; set; }

		[DataMember]
		public Photo Photo { get; set; }

		[DataMember]
		public Guid AdditionalColumnTypeUID { get; set; }

		[DataMember]
		public AdditionalColumnDataType DataType { get; set; }

		[DataMember]
		public string ColumnName { get; set; }

		[DataMember]
		public string TextData { get; set; }
	}
}