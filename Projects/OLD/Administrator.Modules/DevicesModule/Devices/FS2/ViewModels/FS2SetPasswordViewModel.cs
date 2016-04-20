//using Controls;
//using FiresecAPI.Models;
//using Infrastructure.Common.Windows.Windows;
//using Infrastructure.Common.Windows.Windows.ViewModels;

//namespace DevicesModule.ViewModels
//{
//	public class FS2SetPasswordViewModel : SaveCancelDialogViewModel
//	{
//		Device Device;
//		static bool IsUsb;

//		public FS2SetPasswordViewModel(Device device, bool isUsb)
//		{
//			Device = device;
//			IsUsb = isUsb;
//			Title = "Смена пароля";
//			DevicePasswordType = DevicePasswordType.Administrator;
//		}

//		DevicePasswordType _devicePasswordType;
//		public DevicePasswordType DevicePasswordType
//		{
//			get { return _devicePasswordType; }
//			set
//			{
//				_devicePasswordType = value;
//				OnPropertyChanged(()=>DevicePasswordType);
//			}
//		}

//		string _password;
//		public string Password
//		{
//			get { return _password; }
//			set
//			{
//				_password = value;
//				OnPropertyChanged(()=>Password);
//			}
//		}

//		string _passwordConfirm;
//		public string PasswordConfirm
//		{
//			get { return _passwordConfirm; }
//			set
//			{
//				_passwordConfirm = value;
//				OnPropertyChanged(()=>PasswordConfirm);
//			}
//		}

//		protected override bool Save()
//		{
//			if (Password == null)
//				Password = "";
//			if (PasswordConfirm == null)
//				PasswordConfirm = "";

//			if ((Password != "") && (PasswordConfirm != ""))
//				if (!DigitalPasswordHelper.Check(Password) || !DigitalPasswordHelper.Check(PasswordConfirm))
//				{
//					MessageBoxService.Show("Пароль может содержать только цифры");
//					return false;
//				}

//			if (Password != PasswordConfirm)
//			{
//				MessageBoxService.Show("Пароль и подтверждение пароля должны совпадать");
//				return false;
//			}

//			FS2SetPasswordHelper.Run(Device, IsUsb, _devicePasswordType, _password);
//			return base.Save();
//		}
//	}
//}