﻿using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class NowArguments
	{
		public NowArguments()
		{
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument ResultArgument { get; set; }
	}
}
