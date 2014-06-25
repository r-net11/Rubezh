using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Documents;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ConditionArguments
	{
		public ConditionArguments()
		{
			Uid = new Guid();
			Conditions = new List<Condition>();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public List<Condition> Conditions { get; set; }
	}
}
