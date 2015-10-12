using System.ComponentModel;
namespace RubezhAPI.Models
{
	public enum ClientType
	{
		[Description("Администратор")]
		Administrator,

		[Description("Оперативная задача")]
		Monitor,

		[Description("Itv")]
		Itv,

		[Description("OPC Сервер")]
		OPC,

		[Description("Веб-сервер")]
		WebService,

		[Description("Другой")]
		Other
	}
}