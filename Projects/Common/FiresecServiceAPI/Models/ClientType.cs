using System.ComponentModel;

namespace StrazhAPI.Models
{
	public enum ClientType
	{
		[Description("Администратор")]
		Administrator,

		[Description("Оперативная задача")]
		Monitor,

		[Description("Монитор сервера")]
		ServiceMonitor
	}
}