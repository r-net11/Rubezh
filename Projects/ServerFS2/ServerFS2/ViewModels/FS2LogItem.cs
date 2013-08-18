using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerFS2.ViewModels
{
	public class FS2LogItem
	{
		public DateTime DateTime { get; set; }
		public string Name { get; set; }
		public int AttemptNo { get; set; }
	}
}