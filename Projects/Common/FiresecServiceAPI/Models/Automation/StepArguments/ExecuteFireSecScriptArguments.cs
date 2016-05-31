using StrazhAPI.Automation.Enums;
using System.Runtime.Serialization;
using StrazhAPI.Integration.OPC;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class ExecuteFireSecScriptArguments
	{
		[DataMember]
		public Script CurrentScript { get; set; }

		[DataMember]
		public FiresecCommandType CommandType { get; set; }


	}
}
