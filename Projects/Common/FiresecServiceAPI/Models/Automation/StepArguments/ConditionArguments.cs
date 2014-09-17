using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ConditionArguments
	{
		public ConditionArguments()
		{
			Conditions = new List<Condition>();
		}

		[DataMember]
		public List<Condition> Conditions { get; set; }

		[DataMember]
		public JoinOperator JoinOperator { get; set; }
	}
}
