
using System.ComponentModel;
namespace ResursAPI
{
	public enum ClassType
	{
		IsNone =0,
		[Description("Система")]
		IsSystem = 1,
		[Description("Пользователь")]
		IsUser = 2,
		[Description("Абонент")]
		IsConsumer = 3,
		[Description("Устройства")]
		IsDevice = 4,
		[Description("Тариф")]
		IsTariff = 5,
	}
}