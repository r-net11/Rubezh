using ChinaSKDDriverAPI;
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

			Doors = password.DoorsCount.ToString() + "(";
			foreach (var door in password.Doors)
			{
				Doors += door.ToString() + ",";
			}
			Doors += ")";
		}

		public string CreationDateTime { get; private set; }
		public string Doors { get; private set; }
	}
}