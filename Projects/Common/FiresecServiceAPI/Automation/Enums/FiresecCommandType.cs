using System.ComponentModel;

namespace StrazhAPI.Automation.Enums
{
	public enum FiresecCommandType
	{
		[Description("Запустить")]
		Run,
		[Description("Остановить")]
		Stop,
		[Description("Разблокировать")]
		Unlock,
		[Description("Блокировать")]
		Lock
	}
}
