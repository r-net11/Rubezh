using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace StrazhAPI.Enums
{
	public enum GuardZoneType
	{
		[Description("Обычная")]
		Normal = 0,
		[Description("Без права снятия")]
		WithoutUnset = 1,
		[Description("С задержкой входа/выхода")]
		WithDelay = 2
	}
}
