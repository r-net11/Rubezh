using StrazhAPI.Enums;
using System.Runtime.Serialization;

namespace StrazhAPI.Integration.OPC
{
	[DataContract]
	public class OPCZone : ModelBase
	{
		[DataMember]
		public OPCZoneType? Type { get; set; }

		[DataMember]
		public GuardZoneType? GuardZoneType { get; set; }

		[DataMember]
		public bool? IsSkippedTypeEnabled { get; set; }

		[DataMember]
		public int? Delay { get; set; }

		[DataMember]
		public int? AutoSet { get; set; }
	}
}
