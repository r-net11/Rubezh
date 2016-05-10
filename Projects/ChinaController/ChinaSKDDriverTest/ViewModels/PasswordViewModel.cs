using StrazhDeviceSDK.API;
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
			for (int i = 0; i < password.DoorsCount; i++)
			{
				Doors += password.Doors[i].ToString();
				if (i < password.DoorsCount - 1)
					Doors += ",";
			}
			Doors += ")";
		}

		public string CreationDateTime { get; private set; }
		public string Doors { get; private set; }
	}
}