namespace Infrastructure.Common.Windows.ViewModels
{
	public sealed class ConnectionViewModel : WindowBaseViewModel
	{
		public ConnectionViewModel(bool restrictClose = true)
		{
			Title = "Соединение с сервером";
			if (AppSettingsManager.IsRemote && !string.IsNullOrEmpty(AppSettingsManager.RemoteAddress))
				Title += "\n" + AppSettingsManager.RemoteAddress;

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
