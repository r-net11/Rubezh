
namespace Infrastructure.Common.Windows.ViewModels
{
	public sealed class ConnectionViewModel : WindowBaseViewModel
	{
		public ConnectionViewModel(bool restrictClose = true)
		{
			Sizable = false;
			TopMost = true;
			RestrictClose = restrictClose;
			HideInTaskbar = true;
		}

		public bool RestrictClose { get; private set; }

		public override bool OnClosing(bool isCanceled)
		{
			return RestrictClose;
		}
		public void ForceClose()
		{
			RestrictClose = false;
			Close();
		}
	}
}
