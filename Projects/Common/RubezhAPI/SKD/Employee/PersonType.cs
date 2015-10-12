using System.ComponentModel;

namespace RubezhAPI.SKD
{
	public enum PersonType
	{
		[DescriptionAttribute("Сотрудник")]
		Employee,

		[DescriptionAttribute("Посетитель")]
		Guest
	}
}