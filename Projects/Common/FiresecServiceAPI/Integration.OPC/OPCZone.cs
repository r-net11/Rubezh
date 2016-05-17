using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI.Enums;
using StrazhAPI.GK;

namespace StrazhAPI.Integration.OPC
{
	public class OPCZone : ModelBase
	{
		public OPCZoneType Type { get; set; }
	}
}
