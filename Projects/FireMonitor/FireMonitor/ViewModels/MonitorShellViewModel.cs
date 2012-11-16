using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Microsoft.Win32;

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
                    RegistryKey saveKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
                    saveKey.SetValue("isException", false);
                    saveKey.Close();
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