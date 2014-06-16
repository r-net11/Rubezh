using ControllerSDK.API;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class PasswordViewModel : BaseViewModel
	{
		public Password Password { get; private set; }

		public PasswordViewModel(Password password)
		{
			Password = password;
			CreationDateTime = password.CreationDateTime.ToString();
		}

		public string CreationDateTime { get; private set; }
	}
}