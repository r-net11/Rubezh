﻿using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class StopRecordArguments
	{
		public StopRecordArguments()
		{
			CameraArgument = new Argument();
			EventUIDArgument = new Argument();
		}

		[DataMember]
		public Argument CameraArgument { get; set; }

		[DataMember]
		public Argument EventUIDArgument { get; set; }

	}
}