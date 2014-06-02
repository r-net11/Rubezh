using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK
{
	public class CardViewModel : BaseViewModel
	{
		public Card Card { get; private set; }

		public CardViewModel(Card card)
		{
			Card = card;
			CreationDateTime = card.CreationDateTime.ToString();
			ValidStartDateTime = card.ValidStartDateTime.ToString();
			ValidEndDateTime = card.ValidEndDateTime.ToString();
		}

		public string CreationDateTime { get; private set; }
		public string ValidStartDateTime { get; private set; }
		public string ValidEndDateTime { get; private set; }
	}
}