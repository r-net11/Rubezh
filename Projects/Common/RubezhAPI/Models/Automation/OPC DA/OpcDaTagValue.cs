using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubezhAPI.Automation
{
	public class OpcDaTagValue: OpcDaTag
	{
		#region Properties
		
		public Object Value { get; set; }
		public Int16 Quality { get; set; }
		public DateTime Timestamp { get; set; }
		public bool OperationResult { get; set; }

		#endregion
	}
}