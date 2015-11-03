using RubezhAPI.Models;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Client.Login.ViewModels;

namespace FireMonitor
{
	public class SecurityService : ISecurityService
	{
		public bool Validate()
		{
			var loginViewModel = new LoginViewModel(ClientType.Monitor, Infrastructure.Client.Login.ViewModels.LoginViewModel.PasswordViewType.Validate) { Title = "Оперативная задача. Авторизация", };
			DialogService.ShowModalWindow(loginViewModel);
			if (!loginViewModel.IsConnected)
			{
				MessageBoxService.Show(loginViewModel.Message);
			}
			return loginViewModel.IsConnected;
		}
	}
}