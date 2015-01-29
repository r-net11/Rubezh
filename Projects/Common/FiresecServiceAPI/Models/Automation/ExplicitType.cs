using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum ExplicitType
	{
		[DescriptionAttribute("Целое")]
		Integer,

		[DescriptionAttribute("Логическое")]
		Boolean,

		[DescriptionAttribute("Дата и время")]
		DateTime,

		[DescriptionAttribute("Строка")]
		String,

		[DescriptionAttribute("Идентификатор")]
		Guid,

		[DescriptionAttribute("Объектная ссылка")]
		Object,

		[DescriptionAttribute("Перечисление")]
		Enum
	}
}