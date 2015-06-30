using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum PersonType
	{
		[DescriptionAttribute("Сотрудник")]
		Employee,

		[DescriptionAttribute("Посетитель")]
		Guest
	}
}