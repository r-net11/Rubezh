using System.ComponentModel;

namespace StrazhAPI.Models
{
	public enum ClientType
	{
		[Description("Администратор")]
		Administrator,

		[Description("Оперативная задача")]
		Monitor,

		[Description("Itv")]
		Itv,

		[Description("Ассад")]
		Assad,

		[Description("Другой")]
		Other
	}
}