using System.ComponentModel;

namespace StrazhAPI.Enums
{
	public enum OPCCommandType
	{
		[Description("Сброс пожаров")]
		ResetFire,
		[Description("Сброс тревог")]
		ResetAlarm
	}
}
