using System.ComponentModel;

namespace RubezhAPI.Models
{
	public enum RviStatus
	{
		[Description("Есть соединение")]
		Connected,
		[Description("Подключается")]
		Connecting,
		[Description("Соединение отсутсвует")]
		ConnectionLost,
		[Description("Ошибка")]
		Error
	}
}