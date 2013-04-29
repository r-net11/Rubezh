using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Binary
{
	public class BinaryZone
	{
		public BinaryZone(Zone zone)
		{
			BinaryPanels = new List<BinaryPanel>();
			Zone = zone;
		}
		public Device ParentPanel { get; set; }
		public Zone Zone { get; set; }
		public int LocalNo { get; set; }
		public List<BinaryPanel> BinaryPanels { get; set; }
		public object TableBase { get; set; }
	}
}