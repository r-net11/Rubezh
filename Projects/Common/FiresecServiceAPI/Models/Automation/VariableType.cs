using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum VariableType
	{
		[DescriptionAttribute("Целое")]
		Integer,

		[DescriptionAttribute("Логическое")]
		Boolean,

		[DescriptionAttribute("Дата и время")]
		DateTime,

		[DescriptionAttribute("Строка")]
		String,

		[DescriptionAttribute("Объектная ссылка")]
		Object
	}
}
