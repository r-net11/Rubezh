using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace GKModuleTest
{
	public class MockDialogService : IDialogService
	{
		public event Action<WindowBaseViewModel> OnShowModal;
		public event Action<WindowBaseViewModel> OnShow;

		public bool ShowModalWindow(WindowBaseViewModel windowBaseViewModel)
		{
			if (OnShowModal != null)
				OnShowModal(windowBaseViewModel);
			return windowBaseViewModel.CloseResult == null ? false : windowBaseViewModel.CloseResult.Value;
		}

		public void ShowWindow(WindowBaseViewModel windowBaseViewModel)
		{
			if (OnShow != null)
				OnShow(windowBaseViewModel);
		}
	}
}