using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using ServerFS2.Processor;

namespace AdministratorTestClientFS2.ViewModels
{
	public class PasswordViewModel : DialogViewModel
	{
		public PasswordViewModel(Device device)
		{
			Title = "Запись пароля";
			selectedDeivce = device;
			SetPasswordCommand = new RelayCommand(OnSetPasswordCommand);
		}
		private Device selectedDeivce;

		private string passWordString;
		public string PasswordString
		{
			get { return passWordString; }
			set
			{
				passWordString = value;
				OnPropertyChanged("PasswordString");
			}
		}

		private DevicePasswordType devicePasswordType = DevicePasswordType.Installator;
		public DevicePasswordType DevicePasswordType
		{
			get { return devicePasswordType; }
			set
			{
				devicePasswordType = value;
				OnPropertyChanged("DevicePasswordType");
			}
		}

		public RelayCommand SetPasswordCommand { get; private set; }
		private void OnSetPasswordCommand()
		{
			MainManager.DeviceSetPassword(selectedDeivce, false, DevicePasswordType, PasswordString, "Тестовый пользователь");
			Close(true);
		}

	}
}
