using System;
using System.Linq;
using System.Windows;
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
		    IsException = false;
		}

        public static bool IsException { get; set; }
		public override bool OnClosing(bool isCanceled)
		{
            if (IsException)
                return false;
            if (!FiresecManager.CheckPermission(PermissionType.Oper_Logout))
            {
                MessageBoxService.Show("Нет прав для выхода из программы");
                return true;
            }
            var result = MessageBoxService.ShowConfirmation("Вы действительно хотите выйти из программы?");
            if (result == MessageBoxResult.No)
                return true;
			if (FiresecManager.CheckPermission(PermissionType.Oper_LogoutWithoutPassword))
				return false;
			return !ServiceFactory.SecurityService.Validate();
		}
		public override void OnClosed()
		{
			base.OnClosed();
		}
	}
}