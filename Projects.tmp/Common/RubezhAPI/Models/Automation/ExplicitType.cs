﻿using System.ComponentModel;

namespace RubezhAPI.Automation
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

		[DescriptionAttribute("Объектная ссылка")]
		Object,

		[DescriptionAttribute("Перечисление")]
		Enum
	}
}