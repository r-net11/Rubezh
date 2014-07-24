using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardViewModel : BaseViewModel
	{
		public SKDCard Card { get; private set; }

		public CardViewModel(SKDCard card)
		{
			Card = card;
		}

		public void Update(SKDCard card)
		{
			Card = card;
			OnPropertyChanged(() => Card);
		}
	}
}