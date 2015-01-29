using System;
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
			UIDArgument = new Argument();
			DurationArgument = new Argument();
		}

		[DataMember]
		public Argument CameraArgument { get; set; }

		[DataMember]
		public Argument UIDArgument { get; set; }

		[DataMember]
		public Argument DurationArgument { get; set; }

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