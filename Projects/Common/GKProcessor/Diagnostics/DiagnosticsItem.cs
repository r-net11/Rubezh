using System;

namespace GKProcessor
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