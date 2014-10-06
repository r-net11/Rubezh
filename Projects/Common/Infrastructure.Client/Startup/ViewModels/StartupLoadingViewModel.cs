using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace Infrastructure.Client.Startup.ViewModels
{
	public class StartupLoadingViewModel : LoadingViewModel
	{
		public StartupLoadingViewModel()
		{
			//AllowClose = false;
		}
		protected override void OnCancel()
		{
			base.OnCancel();
			ApplicationService.Invoke((Action)(() => { throw new StartupCancellationException(); }));
		}
	}
}
