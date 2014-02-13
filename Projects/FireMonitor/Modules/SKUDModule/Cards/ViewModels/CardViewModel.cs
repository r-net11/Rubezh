using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

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
			OnPropertyChanged("Card");
		}
	}
}