using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum CardType
	{
		[Description("Постоянный")]
		Constant,

		[Description("Временный")]
		Temporary,

		[Description("Гостевой")]
		Guest,

		[Description("Заблокирован")]
		Blocked,

		[Description("Принуждение")]
		Duress
	}
}