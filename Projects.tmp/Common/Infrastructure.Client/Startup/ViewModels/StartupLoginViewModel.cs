using RubezhAPI.Models;
using Infrastructure.Client.Login.ViewModels;

namespace Infrastructure.Client.Startup.ViewModels
{
	internal class StartupLoginViewModel : LoginViewModel
	{
		public StartupLoginViewModel(ClientType clientType)
			: base(clientType)
		{
		}

		protected override bool Save()
		{
			return true;
		}
		protected override bool Cancel()
		{
			return false;
		}
	}
}