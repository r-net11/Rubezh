﻿using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum DocumentType
	{
		[Description("Переработка")]
		Overtime = 0,

		[Description("Присутствие")]
		Presence = 1,

		[Description("Отсутствие")]
		Absence = 2,
	}
}