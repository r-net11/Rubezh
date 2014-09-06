using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class SoundArguments
	{
		public SoundArguments()
		{
			SoundUid = new Guid();
			ProcedureLayoutCollection = new ProcedureLayoutCollection();
		}

		[DataMember]
		public Guid SoundUid { get; set; }

		[DataMember]
		public ProcedureLayoutCollection ProcedureLayoutCollection { get; set; }
	}
}