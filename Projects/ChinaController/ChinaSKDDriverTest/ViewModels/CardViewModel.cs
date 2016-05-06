using StrazhDeviceSDK.API;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class CardViewModel : BaseViewModel
	{
		public Card Card { get; private set; }

		public CardViewModel(Card card)
		{
			Card = card;
			ValidStartDateTime = card.ValidStartDateTime.ToString();
			ValidEndDateTime = card.ValidEndDateTime.ToString();

			Doors = card.DoorsCount.ToString() + "(";
			for (int i = 0; i < card.DoorsCount; i++)
			{
				Doors += card.Doors[i].ToString();
				if(i < card.DoorsCount - 1)
					Doors += ",";
			}
			Doors += ")";

			TimeSections = card.TimeSectionsCount.ToString() + "(";
			for (int i = 0; i < card.TimeSectionsCount; i++)
			{
				TimeSections += card.TimeSections[i].ToString();
				if (i < card.TimeSectionsCount - 1)
					TimeSections += ",";
			}
			TimeSections += ")";
		}

		public string ValidStartDateTime { get; private set; }
		public string ValidEndDateTime { get; private set; }
		public string Doors { get; private set; }
		public string TimeSections { get; private set; }
	}
}