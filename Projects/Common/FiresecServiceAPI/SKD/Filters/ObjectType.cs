using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum ObjectType
	{
		[DescriptionAttribute("Устройство ГК")]
		Устройство_ГК,

		[DescriptionAttribute("Зона ГК")]
		Зона_ГК,

		[DescriptionAttribute("Направление ГК")]
		Направление_ГК,

		[DescriptionAttribute("Задержка ГК")]
		Задержка_ГК,

		[DescriptionAttribute("МПТ")]
		МПТ,

		[DescriptionAttribute("НС ГК")]
		НС_ГК,

		[DescriptionAttribute("Устройство СКД")]
		Устройство_СКД,

		[DescriptionAttribute("Видеоустройство")]
		Видеоустройство,

		[DescriptionAttribute("Система")]
		Система,

		[DescriptionAttribute("Контроллер СКД")]
		Контроллер_СКД,

		[DescriptionAttribute("Считыватель СКД")]
		Считыватель_СКД,
	}
}