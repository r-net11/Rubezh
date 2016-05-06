using System;
using System.Diagnostics;
using System.Windows;
using Common;
using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace FireMonitor.ViewModels
{
	public class MonitorShellViewModel : ShellViewModel
	{
		public MonitorShellViewModel()
			: this(ClientType.Monitor)
		{
		}
		protected MonitorShellViewModel(ClientType clientType)
			: base(clientType)
		{
			Title ="Оперативная задача";
			Toolbar = new ToolbarViewModel();
			Height = 700;
			Width = 1100;
			MinWidth = 980;
			MinHeight = 550;
			UptateOnChangeViewPermission();
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Subscribe((x) => { UptateOnChangeViewPermission(); });
		}

		void UptateOnChangeViewPermission()
		{
			var hasPermission = !ShellIntegrationHelper.IsIntegrated && FiresecManager.CheckPermission(PermissionType.Oper_ChangeView);
			AllowMaximize = hasPermission;
			AllowMinimize = hasPermission;
			Sizable = hasPermission;
		}

		public override bool OnClosing(bool isCanceled)
		{
			if (base.OnClosing(isCanceled))
				return true;
			if (((App)Application.Current).IsClosingOnException)
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
			if (!ApplicationService.IsShuttingDown)
			{
				if (!MessageBoxService.ShowConfirmation("Вы действительно хотите выйти из программы?"))
					return true;
			}
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

		public override void Run()
		{
			base.Run();
		}
	}
}