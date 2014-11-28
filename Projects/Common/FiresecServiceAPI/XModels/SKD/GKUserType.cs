using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum GKUserType
	{
		[Description("Сотрудник")]
		Employee,

		[Description("Оператор")]
		Operator,

		[Description("Администратор")]
		Administrator,

		[Description("Инсталлятор")]
		Installator,

		[Description("Изготовитель")]
		Manufactor
	}
}