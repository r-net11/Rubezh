using System;
using System.Diagnostics;
using System.Windows;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;
using Infrastructure.Common;

namespace FireMonitor.ViewModels
{
	public class MonitorShellViewModel : ShellViewModel
	{
		public MonitorShellViewModel()
		{
			Title = "Оперативная задача ОПС FireSec-2";
			Toolbar = new ToolbarViewModel();
			//ContentFotter = new UserFotterViewModel();
			Height = 700;
			Width = 1100;
			MinWidth = 980;
			MinHeight = 550;
			//HideInTaskbar = App.IsMulticlient;
			if (ShellIntegrationHelper.IsIntegrated)
			{
				AllowMaximize = false;
				AllowMinimize = false;
				Sizable = false;
			}
		}

		public override bool OnClosing(bool isCanceled)
		{
			if (App.IsMulticlient)
			{
				isCanceled = true;
				return true;
			}

			if (App.IsClosingOnException)
			{
#if DEBUG
				return false;
#endif
				Process.GetCurrentProcess().Kill();
				return false;
			}
			if (!FiresecManager.CheckPermission(PermissionType.Oper_Logout))
			{
				MessageBoxService.Show("Нет прав для выхода из программы");
				return true;
			}
			var result = MessageBoxService.ShowConfirmation("Вы действительно хотите выйти из программы?");
			if (result == MessageBoxResult.No)
				return true;
			if (FiresecManager.CheckPermission(PermissionType.Oper_LogoutWithoutPassword))
			{
				try
				{
					RegistrySettingsHelper.SetBool("isException", false);
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "MonitorShellViewModel.OnClosing");
				}
				return false;
			}
			return !ServiceFactory.SecurityService.Validate();
		}
		public override void OnClosed()
		{
			base.OnClosed();
		}
	}
}