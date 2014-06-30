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
			Uid = new Guid();
			FindObjectConditions = new List<FindObjectCondition>();
			ResultUid = new Guid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public List<FindObjectCondition> FindObjectConditions { get; set; }

		[DataMember]
		public JoinOperator JoinOperator { get; set; }

		[DataMember]
		public Guid ResultUid { get; set; }
	}
}