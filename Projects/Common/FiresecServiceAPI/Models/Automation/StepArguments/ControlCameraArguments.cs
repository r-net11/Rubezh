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
			Uid = Guid.NewGuid();
			Variable1 = new ArithmeticParameter();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public ArithmeticParameter Variable1 { get; set; }

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