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
		Fire,
		[Description("Охранная")]
		Guard
	}
}
