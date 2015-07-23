using System.ComponentModel;

namespace GKSDK
{
	public enum DeviceControlRegime
	{
		[Description("автоматика")]
		Automatic,

		[Description("ручное")]
		Manual,

		[Description("отключение")]
		Ignore
	}
}
