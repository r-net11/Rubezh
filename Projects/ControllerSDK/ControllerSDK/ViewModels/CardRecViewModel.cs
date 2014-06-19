using ControllerSDK.API;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class CardRecViewModel : BaseViewModel
	{
		public CardRec CardRec { get; private set; }

		public CardRecViewModel(CardRec cardRec)
		{
			CardRec = cardRec;
			CreationDateTime = cardRec.DateTime.ToString();
		}

		public string CreationDateTime { get; private set; }
	}
}