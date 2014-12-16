using FiresecAPI.SKD;
using Infrastructure.Common.CheckBoxList;

namespace SKDModule.ViewModels
{
	public class CardTypeViewModel : CheckBoxItemViewModel
	{
		public CardType CardType { get; private set; }

		public CardTypeViewModel(CardType cardType)
		{
			CardType = cardType;
		}
	}
}
