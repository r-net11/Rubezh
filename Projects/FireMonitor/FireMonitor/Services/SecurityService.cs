using RubezhAPI.Models;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Windows.Services;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Client.Login.ViewModels;

namespace FireMonitor
{
	public class SecurityService : ISecurityService
	{
		public bool Validate(bool flag = true)
		{
			if (flag && ClientManager.CheckPermission(PermissionType.Oper_MayNotConfirmCommands))
			{
					return true;
			}
			else
			{
				var loginViewModel = new LoginViewModel(ClientType.Monitor, Infrastructure.Client.Login.ViewModels.LoginViewModel.PasswordViewType.Validate) { Title = "Оперативная задача. Авторизация", };
				DialogService.ShowModalWindow(loginViewModel);
				if (!loginViewModel.IsConnected && !loginViewModel.IsCanceled)
				{
					MessageBoxService.ShowError(loginViewModel.Message);
				}
				return loginViewModel.IsConnected;
			}
		}
	}
}