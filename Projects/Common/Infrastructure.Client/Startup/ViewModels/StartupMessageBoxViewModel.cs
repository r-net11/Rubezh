﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using System.Windows;
using Infrastructure.Common.Windows;

namespace Infrastructure.Client.Startup.ViewModels
{
	public class StartupMessageBoxViewModel : MessageBoxViewModel
	{
		public StartupMessageBoxViewModel(string title, string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage, bool isException = false)
			: base(title, message, messageBoxButton, messageBoxImage, isException)
		{
		}

		public override void OnLoad()
		{
			if (StartupService.Instance.IsActive)
			{
				Surface.Owner = StartupService.Instance.OwnerWindow;
				Surface.ShowInTaskbar = false;
				Surface.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}
			else
				base.OnLoad();
		}
		public override int GetPreferedMonitor()
		{
			if (StartupService.Instance.IsActive)
				return MonitorHelper.FindMonitor(StartupService.Instance.OwnerWindow.RestoreBounds);
			else
				return base.GetPreferedMonitor();
		}
	}
}