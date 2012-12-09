using System;
using System.Diagnostics;

namespace Common
{
	public class TimeCounter : IDisposable
	{
		public string Label { get; private set; }
		public DateTime Start { get; private set; }
		public DateTime End { get; private set; }
		public TimeSpan Period { get; private set; }

		public TimeCounter(string label = "{0}")
		{
			Start = DateTime.Now;
			Label = label;
		}

		#region IDisposable Members

		public void Dispose()
		{
			End = DateTime.Now;
			Period = End - Start;
			Debug.WriteLine(string.Format(Label, Period, Start, End));
		}

		#endregion
	}
}
