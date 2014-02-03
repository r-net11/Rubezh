using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecAPI;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class CardsViewModel : ViewPartViewModel
	{
		public CardsViewModel()
		{
		}

		ObservableCollection<SKDCard> _cards;
		public ObservableCollection<SKDCard> Cards
		{
			get { return _cards; }
			set
			{
				_cards = value;
				OnPropertyChanged("Cards");
			}
		}

		SKDCard _selectedCard;
		public SKDCard SelectedCard
		{
			get { return _selectedCard; }
			set
			{
				_selectedCard = value;
				OnPropertyChanged("SelectedCard");
			}
		}
	}
}