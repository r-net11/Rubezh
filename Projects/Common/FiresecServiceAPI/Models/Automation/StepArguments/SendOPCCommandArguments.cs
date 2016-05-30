using StrazhAPI.Enums;
using System.Runtime.Serialization;

namespace StrazhAPI.Models.Automation.StepArguments
{
	[DataContract]
	public class SendOPCCommandArguments
	{
		[DataMember]
		public OPCCommandType SelectedCommandType { get; set; }
	}
}
