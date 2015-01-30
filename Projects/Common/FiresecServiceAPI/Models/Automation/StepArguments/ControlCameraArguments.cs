﻿using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ControlCameraArguments
	{
		public ControlCameraArguments()
		{
			CameraArgument = new Argument();
			EventUIDArgument = new Argument();
			TimeoutArgument = new Argument();
		}

		[DataMember]
		public Argument CameraArgument { get; set; }

		[DataMember]
		public Argument EventUIDArgument { get; set; }

		[DataMember]
		public Argument TimeoutArgument { get; set; }

		[DataMember]
		public CameraCommandType CameraCommandType { get; set; }
	}

	public enum CameraCommandType
	{
		[Description("Начать запись")]
		StartRecord,

		[Description("Остановить запись")]
		StopRecord
	}
}