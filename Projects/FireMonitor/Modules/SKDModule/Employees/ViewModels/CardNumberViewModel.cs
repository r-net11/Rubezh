using Infrastructure.Common.Windows.ViewModels;
using Localization.SKD.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardNumberViewModel : SaveCancelDialogViewModel
	{
		public CardNumberViewModel()
		{
			Title = CommonViewModels.EnterCardNumber;
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