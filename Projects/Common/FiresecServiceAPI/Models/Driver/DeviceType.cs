using System.ComponentModel;

namespace FiresecAPI.Models
{
	public enum DeviceType
	{
		[DescriptionAttribute("Пожарное")]
		Fire,

		[DescriptionAttribute("Охранное")]
		Sequrity,

		[DescriptionAttribute("Технологическое")]
		Technoligical,

		[DescriptionAttribute("Охранно-пожарное")]
		FireSecurity
	}
}