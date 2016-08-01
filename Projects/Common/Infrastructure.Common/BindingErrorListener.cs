using System;
using System.Diagnostics;

namespace Infrastructure.Common
{
	public class BindingErrorListener : TraceListener
	{
		private Action<string> _logAction;

		public static void Listen(Action<string> logAction)
		{
			PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorListener { _logAction = logAction });
		}

		public override void Write(string message)
		{
		}

		public override void WriteLine(string message)
		{
			_logAction(message);
		}
	}
}