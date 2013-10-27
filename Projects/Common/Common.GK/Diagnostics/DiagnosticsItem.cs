using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.GK
{
	public class DiagnosticsItem
	{
		public DiagnosticsItem(string name)
		{
			DateTime = DateTime.Now;
			Name = name;
		}

		public DateTime DateTime { get; private set; }
		public string Name { get; private set; }
	}
}