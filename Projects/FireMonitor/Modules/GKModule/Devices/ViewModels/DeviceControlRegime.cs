using System.ComponentModel;

namespace GKModule
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