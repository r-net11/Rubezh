using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class FindObjectArguments
	{
		public FindObjectArguments()
		{
			FindObjectConditions = new List<FindObjectCondition>();
			ResultArgument = new Argument();
		}

		[DataMember]
		public List<FindObjectCondition> FindObjectConditions { get; set; }

		[DataMember]
		public JoinOperator JoinOperator { get; set; }

		[DataMember]
		public Argument ResultArgument { get; set; }
	}
}