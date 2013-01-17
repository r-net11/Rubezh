using System;
using System.Windows.Input;

namespace Infrastructure.Common
{
	public class WaitWrapper : IDisposable
	{
		public WaitWrapper()
		{
			Mouse.OverrideCursor = Cursors.Wait;
		}

		#region IDisposable Members

		public void Dispose()
		{
			Mouse.OverrideCursor = null;
		}

		#endregion
	}
}