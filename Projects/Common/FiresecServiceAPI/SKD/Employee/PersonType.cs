using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum PersonType
	{
		[DescriptionAttribute("Работник")]
		Employee,

		[DescriptionAttribute("Гость")]
		Guest
	}
}