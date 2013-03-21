using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Binary
{
	public class BinaryDirection
	{
		public BinaryDirection()
		{
			//BinaryPanels = new List<BinaryPanel>();
		}
		public Device ParentPanel { get; set; }
		public Direction Direction { get; set; }
		public int LocalNo { get; set; }
		//public List<BinaryPanel> BinaryPanels { get; set; }
		public object TableBase { get; set; }
	}
}