using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum ExplicitType
	{
		[Description("Целое")]
		Integer,

		[Description("Логическое")]
		Boolean,

		[Description("Дата и время")]
		DateTime,

		[Description("Время")]
		Time,

		[Description("Строка")]
		String,

		[Description("Объектная ссылка")]
		Object,

		[Description("Перечисление")]
		Enum
	}
}