using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class SoundArguments
	{
		public SoundArguments()
		{
			
		}

		[DataMember]
		public Guid SoundUid { get; set; }
	}
}
