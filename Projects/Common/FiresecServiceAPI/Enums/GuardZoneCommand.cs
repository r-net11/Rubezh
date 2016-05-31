using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace StrazhAPI.Enums
{
	public enum GuardZoneCommand
	{
		[Description("Поставить на охрану")]
		SetGuard,

		[Description("Снять с охраны")]
		UnsetGuard
	}
}
