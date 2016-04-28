namespace Infrastructure.Common.Windows.ViewModels
{
	public sealed class ConnectionViewModel : WindowBaseViewModel
	{
		public ConnectionViewModel(bool restrictClose = true)
		{
			Title = Resources.Language.Windows.ViewModels.ConnectionViewModel.ConnectionViewModel_Title;
			if (ConnectionSettingsManager.IsRemote && !string.IsNullOrEmpty(ConnectionSettingsManager.RemoteAddress))
				Title += "\n" + ConnectionSettingsManager.RemoteAddress;

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