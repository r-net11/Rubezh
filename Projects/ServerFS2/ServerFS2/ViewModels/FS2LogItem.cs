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
		public string DeviceName { get; set; }
		public int AttemptNo { get; set; }

		public string PresentationDateTime
		{
			get { return DateTime.TimeOfDay.ToString(); }
		}
	}
}