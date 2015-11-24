using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKModuleTest
{
	public class FakeDialogService : IDialogService
	{
		public Action<WindowBaseViewModel> OnShow;

		public bool ShowModalWindow(WindowBaseViewModel windowBaseViewModel)
		{
			if (OnShow != null)
				OnShow(windowBaseViewModel);

			(windowBaseViewModel as SaveCancelDialogViewModel).SaveCommand.Execute();
			return true;
		}

		public void ShowWindow(WindowBaseViewModel windowBaseViewModel)
		{
		}
	}
}