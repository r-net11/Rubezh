using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ProcedureObject
	{
		public ProcedureObject()
		{
			Uid = Guid.NewGuid();
			ObjectUid = new Guid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid ObjectUid { get; set; }
	}
}