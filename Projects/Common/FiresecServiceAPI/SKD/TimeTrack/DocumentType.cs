using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

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