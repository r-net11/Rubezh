using System.ComponentModel;

namespace Infrastructure.Common.Services.Layout
{
	public enum LayoutPartPropertyType
	{
		[Description("Целое")]
		Integer,
		[Description("Вещественное")]
		Double,
		[Description("Логическое")]
		Boolean,
		[Description("Дата и время")]
		DateTime,
		[Description("Строка")]
		String,
		[Description("Объект")]
		Object,
	}
}
