using System.ComponentModel;

namespace FiresecAPI.Models
{
	public enum ZoneType
	{
		[DescriptionAttribute("Пожарная")]
		Fire,

		[DescriptionAttribute("Охранная")]
		Guard
	}
}