using Infrastructure.Common.Windows.ViewModels;
using KeyGenerator;

namespace FiresecService.ViewModels
{
	public class RegistrationViewModel : SaveCancelDialogViewModel
	{
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

		public RegistrationViewModel()
		{
			UserKey = Generator.GetSerialKey();
		}

		//protected override bool Save()
		//{
		//	return true;
		//}
	}
}
