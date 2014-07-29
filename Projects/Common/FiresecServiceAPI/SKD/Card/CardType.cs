using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum CardType
	{
		[Description("Постоянный")]
		Constant,

		[Description("Временный")]
		Temporary,

		[Description("Разовый")]
		OneTime,

		[Description("Заблокирован")]
		Blocked,

		[Description("Принуждение")]
		Dures
	}
}