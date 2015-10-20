using System.ComponentModel;
namespace RubezhAPI.Models
{
	public enum ClientType
	{
		[Description("Администратор")]
		Administrator = 0,

		[Description("Оперативная задача")]
		Monitor = 1,

		[Description("OPC Сервер")]
		OPC = 2,

		[Description("Веб-сервер")]
		WebService = 3,

		[Description("Другой")]
		Other = 4
	}
}