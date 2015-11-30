using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows.Views;
using Infrastructure.Common.Windows;

namespace Infrastructure.Services
{
	public class MonitorDialogService : IDialogService
	{
		public bool ShowModalWindow(WindowBaseViewModel windowBaseViewModel)
		{
			return DialogService.ShowModalWindow(windowBaseViewModel);
		}

		public void ShowWindow(WindowBaseViewModel windowBaseViewModel)
		{
			DialogService.ShowWindow(windowBaseViewModel);
		}
	}
}