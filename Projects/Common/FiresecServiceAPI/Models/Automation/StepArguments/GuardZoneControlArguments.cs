using StrazhAPI.Enums;
using StrazhAPI.Integration.OPC;
using System.Runtime.Serialization;

namespace StrazhAPI.Models.Automation.StepArguments
{
	[DataContract]
	public class GuardZoneControlArguments
	{
		[DataMember]
		public OPCZone CurrentGuardZone { get; set; }

		[DataMember]
		public GuardZoneCommand CommandType { get; set; }
	}
}
