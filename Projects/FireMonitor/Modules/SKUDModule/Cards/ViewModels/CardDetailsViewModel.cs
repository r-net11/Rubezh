using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class CardDetailsViewModel : SaveCancelDialogViewModel
	{
		CardsViewModel CardsViewModel;
		bool IsNew;
		public SKDCard Card { get; private set; }

		public CardDetailsViewModel(CardsViewModel cardsViewModel, SKDCard card = null)
		{
			CardsViewModel = cardsViewModel;
			if (card == null)
			{
				Title = "Создание карты";
				IsNew = true;
				card = new SKDCard();
			}
			else
			{
				Title = string.Format("Свойства карты: {0}", card.Series + "." + card.Number);
				IsNew = false;
			}
			Card = card;
			CopyProperties();
		}

		public void CopyProperties()
		{
			Series = Card.Series;
			Number = Card.Number;
		}

		int _series;
		public int Series
		{
			get { return _series; }
			set
			{
				if (_series != value)
				{
					_series = value;
					OnPropertyChanged("Series");
				}
			}
		}

		int _number;
		public int Number
		{
			get { return _number; }
			set
			{
				if (_number != value)
				{
					_number = value;
					OnPropertyChanged("Number");
				}
			}
		}

		protected override bool CanSave()
		{
			return true;
		}

		protected override bool Save()
		{
			if (CardsViewModel.Cards.Any(x => x.Card.Series == Series && x.Card.Number == Number && x.Card.UID != Card.UID))
			{
				//MessageBoxService.ShowWarning("Серия и номер карты совпадает с введеннымы ранее");
				//return false;
			}

			Card.Series = Series;
			Card.Number = Number;
			return true;
		}
	}
}