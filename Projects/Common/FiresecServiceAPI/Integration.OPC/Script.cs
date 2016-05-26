using System.Runtime.Serialization;

namespace StrazhAPI.Integration.OPC
{
	[DataContract]
	public class Script
	{
		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public bool IsEnabled { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }
	}
}
