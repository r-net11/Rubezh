using System.ComponentModel;

namespace Infrastructure.Models
{
	public enum JournalColumnType
	{
		[DescriptionAttribute("Подсистема")]
		SubsystemType,

		[DescriptionAttribute("Пользователь")]
		UserName
	}
}