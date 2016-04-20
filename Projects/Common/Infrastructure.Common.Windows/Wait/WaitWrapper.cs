using System;
using System.Windows.Input;

namespace Infrastructure.Common.Windows
{
	public class WaitWrapper : IDisposable
	{
		private Cursor _previous;
		public WaitWrapper(bool restore = true)
		{
			_previous = restore ? Mouse.OverrideCursor : null;
			Mouse.OverrideCursor = Cursors.Wait;
		}

		#region IDisposable Members

		public void Dispose()
		{
			Mouse.OverrideCursor = _previous;
		}

		#endregion
	}
}