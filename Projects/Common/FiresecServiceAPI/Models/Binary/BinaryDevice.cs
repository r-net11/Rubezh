using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Binary
{
	public class BinaryDevice
	{
		public BinaryDevice()
		{
			BinaryZones = new List<BinaryZone>();
		}

		public Device ParentPanel { get; set; }
		public Device Device { get; set; }
		public List<BinaryZone> BinaryZones { get; set; }
		public object TableBase { get; set; }
		public BinaryPanel BinaryPanel { get; set; }
	}
}