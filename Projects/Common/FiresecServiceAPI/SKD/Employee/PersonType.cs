using System.ComponentModel;

namespace FiresecAPI
{
	public enum PersonType
	{
		[DescriptionAttribute("Работник")]
		Employee,

		[DescriptionAttribute("Гость")]
		Guest
	}
}