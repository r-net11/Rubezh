using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ForeachArguments
	{
		public ForeachArguments()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid ListVariableUid { get; set; }

		[DataMember]
		public Guid ItemVariableUid { get; set; }
	}
}
