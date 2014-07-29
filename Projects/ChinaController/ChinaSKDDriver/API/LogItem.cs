using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChinaSKDDriverAPI
{
	public class LogItem
	{
		public DateTime DateTime { get; set; }
		public string UserName { get; set; }
		public string LogType { get; set; }
		public string LogMessage { get; set; }
		public string CardId { get; set; }
		public string DoorNo { get; set; }
		public string Type { get; set; }
	}
}