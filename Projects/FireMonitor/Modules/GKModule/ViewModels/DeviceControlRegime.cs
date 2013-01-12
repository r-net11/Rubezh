using System.ComponentModel;

namespace GKModule
{
	public enum DeviceControlRegime
	{
		[Description("Автоматика")]
		Automatic,

		[Description("Ручное")]
		Manual,

		[Description("Отключение")]
		Ignore
	}
}