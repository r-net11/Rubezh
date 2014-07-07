using System.ComponentModel;

namespace FiresecAPI
{
	public enum CameraType
	{
		[Description("Камера")]
		Camera = 0,

		[Description("Видеорегистратор")]
		Dvr = 1,

		[Description("Канал")]
		Channel = 2
	}
}