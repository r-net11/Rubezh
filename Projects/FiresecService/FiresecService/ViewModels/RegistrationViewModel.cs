using Infrastructure.Common.Windows.ViewModels;
using KeyGenerator;
using System.Windows;

namespace FiresecService.ViewModels
{
	public class RegistrationViewModel : SaveCancelDialogViewModel
	{
		private readonly ILicenseManager _licenseService;
		private string _userKey;

		public string UserKey
		{
			get { return _userKey; }
			set
			{
				_userKey = value;
				OnPropertyChanged(() => UserKey);
			}
		}

		private string _productKey;

		public string ProductKey
		{
			get { return _productKey; }
			set
			{
				_productKey = value;
				OnPropertyChanged(() => ProductKey);
			}
		}

		public RegistrationViewModel(ILicenseManager licenseService)
		{
			_licenseService = licenseService;
			UserKey = _licenseService.GetUserKey();
		}

		protected override bool Save()
		{
			if (_licenseService.VerifyProductKey(ProductKey))
			{
				_licenseService.SaveToFile(ProductKey, UserKey);

				MessageBox.Show("Регистрация прошла успешно. Перезапустите приложение для того, что бы изменения вступили в силу.");
				Bootstrapper.Close();
			}
			else
				MessageBox.Show("Неверный ключ продукта.");

			return true;
		}
	}
}
