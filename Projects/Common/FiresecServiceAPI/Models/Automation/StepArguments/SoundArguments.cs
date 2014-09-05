using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class SoundArguments
	{
		public SoundArguments()
		{
			SoundUid = new Guid();
			LayoutsUids = new List<Guid>();
		}

		[DataMember]
		public Guid SoundUid { get; set; }

		[DataMember]
		public List<Guid> LayoutsUids { get; set; }
	}
}