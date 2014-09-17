using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class FindObjectArguments
	{
		public FindObjectArguments()
		{
			FindObjectConditions = new List<FindObjectCondition>();
			ResultParameter = new ArithmeticParameter();
		}

		[DataMember]
		public List<FindObjectCondition> FindObjectConditions { get; set; }

		[DataMember]
		public JoinOperator JoinOperator { get; set; }

		[DataMember]
		public ArithmeticParameter ResultParameter { get; set; }
	}
}