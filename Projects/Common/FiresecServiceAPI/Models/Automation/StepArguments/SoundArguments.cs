using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class SoundArguments
	{
		public SoundArguments()
		{
			SoundUid = new Guid();
		}

		[DataMember]
		public Guid SoundUid { get; set; }
	}
}
