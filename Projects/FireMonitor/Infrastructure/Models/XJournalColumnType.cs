using System.ComponentModel;

namespace Infrastructure.Models
{
	public enum XJournalColumnType
	{
		[DescriptionAttribute("IP-адрес ГК")]
		GKIpAddress = 0,

		[DescriptionAttribute("Подсистема")]
		SubsystemType = 1,

		[DescriptionAttribute("Пользователь")]
		UserName = 2
	}
}