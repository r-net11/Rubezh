using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace Infrastructure.Client.Startup.ViewModels
{
	public class StartupLoadingViewModel : LoadingViewModel
	{
		public StartupLoadingViewModel()
		{
			CanCancel = false;
		}
		protected override void OnCancel()
		{
			base.OnCancel();
			ApplicationService.Invoke((Action)(() => { throw new StartupCancellationException(); }));
		}
	}
}