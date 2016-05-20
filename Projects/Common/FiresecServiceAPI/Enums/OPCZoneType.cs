using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace StrazhAPI.Enums
{
	public enum OPCZoneType
	{
		[Description("Пожарная")]
		Fire = 0,
		[Description("Охранная")]
		Guard = 1,
		[Description("СКУД")]
		ASC = 2
	}
}
