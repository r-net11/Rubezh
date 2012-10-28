using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace FireMonitor.ViewModels
{
	public class MonitorShellViewModel : ShellViewModel
	{
		public MonitorShellViewModel()
		{
			Title = "Оперативная задача ОПС FireSec-2";
			Toolbar = new ToolbarViewModel();
			ContentFotter = new UserFotterViewModel();
			Height = 700;
			Width = 1100;
            MinWidth = 980;
			MinHeight = 550;
		}

		public override bool OnClosing(bool isCanceled)
		{
			if (FiresecManager.CheckPermission(PermissionType.Oper_LogoutWithoutPassword))
				return false;
			if (!FiresecManager.CheckPermission(PermissionType.Oper_Logout))
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