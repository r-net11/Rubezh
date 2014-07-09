using System.ComponentModel;

namespace FiresecAPI.Models
{
	public enum FS1SubsystemType
	{
		[DescriptionAttribute("Прочие")]
		Other = 0,

		[DescriptionAttribute("Пожарная")]
		Fire = 1,

		[DescriptionAttribute("Охранная")]
		Guard = 2
	};
}