using ChinaSKDDriverAPI;
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
			foreach (var door in card.Doors)
			{
				Doors += door.ToString() + ",";
			}
			Doors += ")";

			TimeSections = card.TimeSectionsCount.ToString() + "(";
			foreach (var timeSection in card.TimeSections)
			{
				TimeSections += timeSection.ToString() + ",";
			}
			TimeSections += ")";
		}

		public string ValidStartDateTime { get; private set; }
		public string ValidEndDateTime { get; private set; }
		public string Doors { get; private set; }
		public string TimeSections { get; private set; }
	}
}