using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardNumberViewModel : SaveCancelDialogViewModel
	{
		public CardNumberViewModel()
		{
			Title = "Ввод номера карты с USB-считывателя";
		}

		int _number;
		public int Number
		{
			get { return _number; }
			set
			{
				_number = value;
				OnPropertyChanged(() => Number);
			}
		}
	}
}