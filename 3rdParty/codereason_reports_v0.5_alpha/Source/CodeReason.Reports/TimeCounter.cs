using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CodeReason.Reports
{
	internal class TimeCounter : IDisposable
	{
		public static bool CounterEnabled = false;
		private const string Separator = "===========================================";
		public string Label { get; private set; }
		public DateTime Start { get; private set; }
		public DateTime End { get; private set; }
		public TimeSpan Period { get; private set; }
		public bool ShowStartSeparator { get; private set; }
		public bool ShowEndSeparator { get; private set; }

		static TimeCounter()
		{
#if DEBUG
			CounterEnabled = true;
#endif
		}
		public TimeCounter(string label = "{0}", bool showStartSeparator = false, bool showEndSeparator = false)
		{
			if (CounterEnabled)
			{
				Start = DateTime.Now;
				Label = label;
				ShowStartSeparator = showStartSeparator;
				ShowEndSeparator = showEndSeparator;
				if (ShowStartSeparator)
					Debug.WriteLine(Separator);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (CounterEnabled)
			{
				End = DateTime.Now;
				Period = End - Start;
				Debug.WriteLine(string.Format(Label, Period, Start, End));
				if (ShowEndSeparator)
					Debug.WriteLine(Separator);
			}
		}

		#endregion
	}
}
