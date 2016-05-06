using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace StrazhAPI.Enums
{
	public enum RoundingType
	{
		[Description("Не используется")]
		None,

		[Description("До минут")]
		RoundToMin,

		[Description("До часов")]
		RoundToHour
	}
}
