using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecService.OPC
{
	public class TagZone
	{
		public int TagId { get; set; }
		public ZoneState ZoneState { get; set; }
	}
}