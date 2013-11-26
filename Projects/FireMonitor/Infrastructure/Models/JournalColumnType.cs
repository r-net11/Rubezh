using System.ComponentModel;

namespace Infrastructure.Models
{
	public enum JournalColumnType
	{
		[DescriptionAttribute("IP-адрес ГК")]
		GKIpAddress = 0,

		[DescriptionAttribute("Подсистема")]
		SubsystemType = 1,

		[DescriptionAttribute("Пользователь")]
		UserName = 2
	}
}