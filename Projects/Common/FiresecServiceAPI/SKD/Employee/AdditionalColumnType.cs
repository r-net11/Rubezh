using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class AdditionalColumnType : OrganizationElementBase
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public DataType DataType { get; set; }
	}

	[Flags]
	public enum DataType
	{
		[Description("Текствовый")]
		Text,
		[Description("Графический")]
		Graphics
	}
}