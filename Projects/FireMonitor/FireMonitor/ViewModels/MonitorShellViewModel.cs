using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using Infrastructure.Common.Windows;
using FiresecAPI.Models;
using Infrastructure;

namespace FireMonitor.ViewModels
{
	public class MonitorShellViewModel : ShellViewModel
	{
		public MonitorShellViewModel()
		{
			Title = "Оперативная задача ОПС FireSec-2";
			Height = 700;
			Width = 1100;
			MinWidth = 800;
			MinHeight = 550;
			Toolbar = new ToolbarViewModel();
			ContentFotter = new UserFotterViewModel();
		}

		public override bool OnClosing(bool isCanceled)
		{
			if (!FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_LogoutWithoutPassword))
				return true;
			if (!FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_Logout))
			{
				MessageBoxService.Show("Нет прав для выхода из программы");
				return true;
			}
			return !ServiceFactory.SecurityService.Validate();
		}
		public override void OnClosed()
		{
			base.OnClosed();
		}
	}
}
