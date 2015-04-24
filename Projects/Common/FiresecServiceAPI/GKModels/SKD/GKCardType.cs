using System.ComponentModel;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Тип пользователя ГК
	/// </summary>
	public enum GKCardType
	{
		[Description("Сотрудник")]
		Employee = 0,

		[Description("Оператор")]
		Operator = 1,

		[Description("Администратор")]
		Administrator = 2,

		[Description("Инсталлятор")]
		Installer = 3,

		[Description("Изготовитель")]
		Manufactor = 4,
	}
}