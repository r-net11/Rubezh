using System.Runtime.Serialization;
using System.ComponentModel;

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

	public enum DataType
	{
		[Description("Текствовый")]
		Text,
		[Description("Графический")]
		Graphics
	}
}