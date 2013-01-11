using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Infrastructure.Common.Windows;
using FireMonitor.Multiclient.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;
using MuliclientAPI;

namespace FireMonitor.Multiclient
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ServiceFactory.Initialize();

			var multiclientViewModel = new MulticlientViewModel();
			ApplicationService.Run(multiclientViewModel, true);

			var passwordViewModel = new PasswordViewModel();
			DialogService.ShowModalWindow(passwordViewModel);
			var password = passwordViewModel.Password;
			if (!string.IsNullOrEmpty(password))
			{
				var multiclientConfiguration = MulticlientConfigurationHelper.LoadConfiguration(password);
				multiclientViewModel.Initialize(multiclientConfiguration);
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			Environment.Exit(0);
		}
	}
}