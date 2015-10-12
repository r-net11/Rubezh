using System;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class SoundArguments : UIArguments
	{
		public SoundArguments()
		{
			SoundUid = new Guid();
		}

		[DataMember]
		public Guid SoundUid { get; set; }
	}
}