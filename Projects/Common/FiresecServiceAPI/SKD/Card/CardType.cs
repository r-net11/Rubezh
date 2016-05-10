using System.ComponentModel;

namespace StrazhAPI.SKD
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