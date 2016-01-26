using System.ComponentModel;
namespace RubezhAPI.Models
{
	public enum ClientType
	{
		[Description("-")]
		None = 0,

		[Description("Администратор")]
		Administrator = 1,

		[Description("Оперативная задача")]
		Monitor = 2,

		[Description("OPC Сервер")]
		OPC = 4,

		[Description("Веб-сервер")]
		WebService = 8,

		[Description("Другой")]
		Other = 16
	}
}