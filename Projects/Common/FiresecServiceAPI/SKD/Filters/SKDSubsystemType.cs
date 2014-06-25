using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum SubsystemType
	{
		[DescriptionAttribute("Система")]
		Система,

		[DescriptionAttribute("Прибор")]
		ГК,

		[DescriptionAttribute("СКД")]
		СКД,

		[DescriptionAttribute("Видео")]
		Видео,
	}
}