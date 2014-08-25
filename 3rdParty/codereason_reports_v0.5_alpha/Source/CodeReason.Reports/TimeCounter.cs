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
		public DateTime Tick { get; private set; }
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
				Tick = Start;
				Label = label;
				ShowStartSeparator = showStartSeparator;
				ShowEndSeparator = showEndSeparator;
				if (ShowStartSeparator)
					Debug.WriteLine(Separator);
			}
		}

		public void ShowTick(string label = "{0}")
		{
			if (CounterEnabled)
			{
				var end = DateTime.Now;
				var period = end - Tick;
				Tick = end;
				Debug.WriteLine(string.Format(label, period, Start, end));
			}
		}
		public void ShowPeriod(string label = "{0}")
		{
			if (CounterEnabled)
			{
				var end = DateTime.Now;
				var period = end - Start;
				Debug.WriteLine(string.Format(label, period, Start, end));
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
